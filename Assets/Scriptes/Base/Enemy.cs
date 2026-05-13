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

    public void Init(EnemyData data)
    {
        configData = data;
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
        moveSpeed = data.moveSpeed;
        damage = data.damage;

        // 初始化时自动获取基地引用
        if (GameManager.Instance != null)
        {
            baseTransform = GameManager.Instance.baseTransform;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  
        wallLayer = LayerMask.GetMask("Wall");
    }

    private void Update()
    {
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
            Base baseComponent = collision.GetComponent<Base>();
            if (baseComponent != null)
            {
                baseComponent.getDamageOfBase(damage);
                WaveManager.Instance?.OnEnemyDied();
                Destroy(gameObject);
            }
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
        WaveManager.Instance.OnEnemyDied();
        GameManager.Instance.updateScord(configData.rewardExp);
        Destroy(gameObject);
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
}