using UnityEngine;

public abstract class ContinuousAttackBullet : Bullet
{
    [Header("持续攻击参数")]
    protected float tickInterval;
    protected int damagePerTick;
    protected Transform target;
    private Transform towerTransform;

    [Header("持续攻击特效偏移")]
    [SerializeField] private Vector3 effectOffset = new Vector3(0, 0.5f, 0);

    private float tickTimer;
    private float maxRange;
    private Vector3 startPoint;
    protected float lineWidth = 0.15f;

    private float outOfRangeTimer = 0f;
    private const float outOfRangeThreshold = 0.2f;

    private GameObject continuousEffect;   // 跟随目标持续播放的特效

    protected override void Awake()
    {
        base.Awake();
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;     // 初始隐藏子弹本体
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

        UpdateLineVisual();
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        // 生成持续跟随特效
        CreateContinuousEffect();

        // 立即伤害一次
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            DealDamageToEnemy(enemy);
            ApplyEffect(enemy);
        }
    }

    protected virtual void Update()
    {
        if (GameManager.IsPaused) return;

        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
            return;
        }

        float distance = Vector2.Distance(towerTransform.position, target.position);
        if (distance > maxRange)
        {
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
            outOfRangeTimer = 0f;
        }

        UpdateLineVisual();

        // 每帧让特效跟随目标位置
        if (continuousEffect != null && target != null)
            continuousEffect.transform.position = target.position + effectOffset;

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

    // 创建持续特效
    private void CreateContinuousEffect()
    {
        if (hitEffect == null || target == null) return;

        Vector3 spawnPos = target.position + effectOffset;
        continuousEffect = Instantiate(hitEffect, spawnPos, hitEffect.transform.rotation);

        var sfx = continuousEffect.GetComponent<SFXPlayerViaManager>();
        if (sfx != null)
        {
            // 持续攻击也可能是循环音效
            sfx.InitAndPlay(clip, true);
        }

        // 粒子系统设置...
    }

    // 销毁持续特效
    private void DestroyContinuousEffect()
    {
        if (continuousEffect != null)
        {
            ParticleSystem ps = continuousEffect.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            Destroy(continuousEffect);
            continuousEffect = null;
        }
    }

    protected virtual void OnDestroy()
    {
        DestroyContinuousEffect();
    }

    protected virtual void ApplyEffect(Enemy enemy) { }
}