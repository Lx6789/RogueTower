using UnityEngine;

public abstract class ContinuousAttackBullet : Bullet
{
    protected float tickInterval;
    protected int damagePerTick;
    protected Transform target;
    private Transform towerTransform;

    private float tickTimer;
    private float maxRange;

    private Vector3 startPoint;
    protected float lineWidth = 0.15f;

    // 脱离范围缓冲
    private float outOfRangeTimer = 0f;
    private const float outOfRangeThreshold = 0.2f; // 允许离开范围 0.2 秒

    protected override void Awake()
    {
        base.Awake();
        if (spriteRenderer != null)
            spriteRenderer.enabled = false; // 初始隐藏，避免原图闪现
    }

    public void SetContinuousParams(Transform target, float maxRange, float tickInterval,
        int damagePerTick, Transform tower)
    {
        this.target = target;
        this.maxRange = maxRange;
        this.tickInterval = tickInterval;
        this.damagePerTick = damagePerTick;
        this.towerTransform = tower;

        tickTimer = 0f;
        damage = damagePerTick;
        startPoint = transform.position;
        outOfRangeTimer = 0f;

        // 先拉伸再显示
        UpdateLineVisual();
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        // 立即伤害
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            DealDamageToEnemy(enemy);
            ApplyEffect(enemy);
        }
    }

    protected virtual void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
            return;
        }

        float distance = Vector2.Distance(towerTransform.position, target.position);
        if (distance > maxRange)
        {
            // 超出范围时累加时间，超过阈值才销毁
            outOfRangeTimer += Time.deltaTime;
            if (outOfRangeTimer >= outOfRangeThreshold)
            {
                if (spriteRenderer != null) spriteRenderer.enabled = false;
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            outOfRangeTimer = 0f; // 回到范围就重置
        }

        UpdateLineVisual();

        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                DealDamageToEnemy(enemy);
                ApplyEffect(enemy);
            }
        }
    }

    private void UpdateLineVisual()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null || target == null) return;

        Vector3 from = startPoint;
        Vector3 to = target.position;
        Vector3 mid = (from + to) * 0.5f;
        Vector3 dir = to - from;
        float length = dir.magnitude;
        if (length < 0.001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.position = mid;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        float scaleX = length / spriteSize.x;
        float scaleY = lineWidth / spriteSize.y;
        transform.localScale = new Vector3(scaleX, scaleY, 1);
    }

    protected virtual void ApplyEffect(Enemy enemy) { }
}