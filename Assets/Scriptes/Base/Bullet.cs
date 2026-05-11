using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [Header("子弹属性")]
    protected int damage;
    protected float speed;
    protected Transform target;
    protected GameObject hitEffect;
    protected LayerMask targetLayer;      // ✅ 目标层（检测什么层）
    protected LayerMask obstacleLayer;    // ✅ 障碍物层（碰到销毁）

    private Rigidbody2D rb;

    /// <summary>
    /// 初始化子弹
    /// </summary>
    public virtual void Init(int damage, float speed, Transform target, GameObject effect,
        LayerMask targetLayer = default, LayerMask obstacleLayer = default)
    {
        this.damage = damage;
        this.speed = speed;
        this.target = target;
        this.hitEffect = effect;
        this.targetLayer = targetLayer;
        this.obstacleLayer = obstacleLayer;
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        MoveTowardsTarget();
    }

    protected virtual void MoveTowardsTarget()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;

        // 检测前方是否有障碍物
        if (obstacleLayer != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, obstacleLayer);
            if (hit.collider != null)
            {
                // 碰到障碍物，销毁子弹
                OnHitObstacle(hit.collider);
                return;
            }
        }

        if (rb != null)
        {
            Vector2 newPos = rb.position + direction * speed * Time.deltaTime;
            rb.MovePosition(newPos);
        }
        else
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// 碰到障碍物
    /// </summary>
    protected virtual void OnHitObstacle(Collider2D obstacle)
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 碰撞检测（碰到敌人）
    /// </summary>
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // 检测是否在目标层
        if (targetLayer != 0 && (targetLayer.value & (1 << collision.gameObject.layer)) == 0)
        {
            return; // 不在目标层，忽略
        }

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}