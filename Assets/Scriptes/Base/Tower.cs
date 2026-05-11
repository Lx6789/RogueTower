using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    private TowerData configData;

    [Header("塔属性（运行时）")]
    protected GameObject bulletPrefab;
    protected float fireRate;
    protected float range;
    protected int damage;
    protected float bulletSpeed;
    protected GameObject hitEffect;

    protected float fireTimer;
    protected Transform currentTarget;

    [Header("检测设置")]
    [SerializeField] private bool lockTarget = true;

    [Header("图层设置")]                                        
    [SerializeField] protected LayerMask obstacleLayer;
    [SerializeField] protected LayerMask enemyLayer;

    void Start()
    {
        if (enemyLayer == 0)
        {
            enemyLayer = LayerMask.GetMask("Enemy");
        }
        if (obstacleLayer == 0)
        {
            obstacleLayer = LayerMask.GetMask("Wall");
        }
        OnStart();
    }

    void Update()
    {
        FindTarget();
        HandleShooting();
        OnUpdate();
    }

    /// <summary>
    /// 初始化塔
    /// </summary>
    public virtual void Init(TowerData data)
    {
        configData = data;
        range = data.range;
        fireRate = data.attackInterval;
        bulletPrefab = data.bulletPrefab;
        damage = data.damage;
        hitEffect = data.BulletEffect;
        bulletSpeed = data.bulletSpeed;
    }

    /// <summary>
    /// 子类 Start
    /// </summary>
    protected virtual void OnStart() { }

    /// <summary>
    /// 子类 Update
    /// </summary>
    protected virtual void OnUpdate() { }

    /// <summary>
    /// 射击（子类实现）
    /// </summary>
    protected abstract void Shoot();

    /// <summary>
    /// 寻找目标
    /// </summary>
    protected virtual void FindTarget()
    {
        if (lockTarget && currentTarget != null)
        {
            float distToTarget = Vector2.Distance(transform.position, currentTarget.position);
            if (distToTarget <= range && currentTarget.gameObject.activeInHierarchy)
            {
                return;
            }
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        float closestDist = Mathf.Infinity;
        Transform newTarget = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    newTarget = hit.transform;
                }
            }
        }

        currentTarget = newTarget;
    }

    /// <summary>
    /// 处理射击逻辑
    /// </summary>
    protected virtual void HandleShooting()
    {
        if (currentTarget == null) return;

        if (!currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;
            return;
        }

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            Shoot();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}