using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Tooltip("射线检测距离")]
    private float rayLength = 0.6f;
    [Tooltip("检测的层级")]
    private LayerMask wallLayer;

    [Header("运行时状态")]
    private int currentHealth;
    private int maxHealth;
    protected float moveSpeed;
    private int damage;
    protected Rigidbody2D rb;
    protected Vector2 moveDirection = Vector2.left;  
    private EnemyData configData;
    protected Transform baseTransform;

    private HealthBarUI healthBarUI;

    public void Init(EnemyData data, int finalMaxHealth)
    {
        configData = data;
        maxHealth = finalMaxHealth;
        currentHealth = maxHealth;
        moveSpeed = data.moveSpeed;
        damage = data.damage;
        moveDirection = Vector2.left;

        // 重置刚体
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // 初始化时自动获取基地引用
        if (GameManager.Instance != null)
        {
            baseTransform = GameManager.Instance.baseTransform;
        }

        // 生成血条
        GameObject barPrefab = GameManager.Instance.healthBarPrefab;
        if (barPrefab != null)
        {
            Canvas worldCanvas = GameObject.Find("Canvas_WorldSpace")?.GetComponent<Canvas>();
            if (worldCanvas != null)
            {
                GameObject barObj = Instantiate(barPrefab, worldCanvas.transform);
                healthBarUI = barObj.GetComponent<HealthBarUI>();
                if (healthBarUI != null)
                {
                    healthBarUI.Init(transform, finalMaxHealth);
                }
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  
        wallLayer = LayerMask.GetMask("Wall");
    }

    private void Update()
    {
        if (GameManager.IsPaused) return;
        DetectWalls();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Base"))
        {
            if (healthBarUI != null)
            {
                Destroy(healthBarUI.gameObject);
                healthBarUI = null;
            }
            Base baseComponent = collision.GetComponent<Base>();
            if (baseComponent != null)
            {
                baseComponent.GetDamageOfBase(damage);
            }
            WaveManager.Instance?.OnEnemyDied();

            ObjectPool.Instance.Release(gameObject, configData.enemyPrefab);
        }
    }

    /// <summary>
    /// 敌人移动（虚方法，子类可重写）
    /// </summary>
    protected virtual void Move()
    {
        Vector2 newPos = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;

            ShowDamageNumber(damage);

            if (healthBarUI != null) healthBarUI.UpdateHealthBar(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    /// <summary>
    /// 敌人死亡
    /// </summary>
    private void Die()
    {
        // 销毁血条
        if (healthBarUI != null)
        {
            Destroy(healthBarUI.gameObject);
            healthBarUI = null;
        }
        
        GameManager.Instance.updateScore(configData.rewardExp);
        GameManager.Instance.updateGold(configData.rewardGold);

        WaveManager.Instance?.OnEnemyDied();

        ObjectPool.Instance.Release(gameObject, configData.enemyPrefab);
    }

    /// <summary>
    /// 检测墙体确定方向
    /// </summary>
    protected virtual void DetectWalls()
    {
        // 四个方向：前、后、左、右
        Vector2 forward = moveDirection;
        Vector2 backward = -moveDirection;
        Vector2 left = Vector2.Perpendicular(moveDirection);
        Vector2 right = -left;

        // 探测四个方向
        bool forwardBlocked = Physics2D.Raycast(transform.position, forward, rayLength, wallLayer);
        bool backwardBlocked = Physics2D.Raycast(transform.position, backward, rayLength, wallLayer);
        bool leftBlocked = Physics2D.Raycast(transform.position, left, rayLength, wallLayer);
        bool rightBlocked = Physics2D.Raycast(transform.position, right, rayLength, wallLayer);

        // 收集可行方向
        List<Vector2> availableDirections = new List<Vector2>();

        if (!forwardBlocked)
            availableDirections.Add(forward);

        if (!leftBlocked)
            availableDirections.Add(left);

        if (!rightBlocked)
            availableDirections.Add(right);

        // 前面三面都是墙才考虑掉头
        if (availableDirections.Count == 0 && !backwardBlocked)
            availableDirections.Add(backward);

        // 从可行方向里选一个
        if (availableDirections.Count > 0)
        {
            if (availableDirections.Contains(forward))
            {
                moveDirection = forward;
            }
            else
            {
                int index = Random.Range(0, availableDirections.Count);
                moveDirection = availableDirections[index];
            }
        }
    }

    private void ShowDamageNumber(int damage)
    {
        // 获取伤害数字预制体引用（可以放在 GameManager 或 EnemyData 里）
        GameObject popupPrefab = GameManager.Instance.damagePopupPrefab; // 需要你在 GameManager 里添加这个字段
        if (popupPrefab == null) return;

        // 计算世界位置（头顶偏移）
        Vector3 popupPos = transform.position + Vector3.up * 1.5f; // 根据你精灵大小调整

        // 从对象池取出
        GameObject popupObj = ObjectPool.Instance.Get(popupPrefab, popupPos, Quaternion.identity);
        DamagePopup popup = popupObj.GetComponent<DamagePopup>();
        if (popup != null)
        {
            Color color = Color.white;
            // 可以根据暴击或伤害类型改颜色，例如：
            // if (isCritical) color = Color.yellow;
            popup.Init(damage, popupPos, color, popupPrefab);
        }
    }
}