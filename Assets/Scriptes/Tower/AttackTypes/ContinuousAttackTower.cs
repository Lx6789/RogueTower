using UnityEngine;
using System.Collections;

public abstract class ContinuousAttackTower : Tower
{
    [Header("持续攻击设置")]
    [SerializeField] protected GameObject beamEffectPrefab;  // 光束特效
    [SerializeField] protected float tickInterval = 0.2f;    // 伤害间隔
    [SerializeField] protected bool requiresLineOfSight = false; // 是否需要视线

    protected GameObject currentBeam;  // 当前光束
    protected float tickTimer;

    protected override void OnUpdate()
    {
        UpdateBeam();
    }

    protected override void Shoot()
    {
        // 持续攻击在 UpdateBeam 中处理
    }

    protected override void HandleShooting()
    {
        // 重写射击逻辑：持续攻击不需要 fireRate 计时
        if (currentTarget == null)
        {
            DestroyBeam();
            return;
        }

        if (!currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;
            DestroyBeam();
            return;
        }

        fireTimer += Time.deltaTime;

        // 开始攻击
        if (fireTimer >= fireRate)
        {
            if (currentBeam == null)
            {
                CreateBeam();
            }
        }
    }

    /// <summary>
    /// 更新光束
    /// </summary>
    protected virtual void UpdateBeam()
    {
        if (currentBeam == null || currentTarget == null) return;

        // 更新光束位置和方向
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, currentTarget.position);

        currentBeam.transform.position = transform.position;
        currentBeam.transform.right = direction;

        // 缩放光束长度
        SpriteRenderer beamRenderer = currentBeam.GetComponent<SpriteRenderer>();
        if (beamRenderer != null)
        {
            beamRenderer.size = new Vector2(distance, beamRenderer.size.y);
        }

        // 持续造成伤害
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;
            ApplyContinuousDamage();
        }

        // 如果超出范围，停止攻击
        if (distance > range)
        {
            StopAttack();
        }
    }

    /// <summary>
    /// 应用持续伤害
    /// </summary>
    protected virtual void ApplyContinuousDamage()
    {
        if (currentTarget != null)
        {
            Enemy enemy = currentTarget.GetComponent<Enemy>();
            if (enemy != null)
            {
                ApplyDamageToEnemy(enemy);
            }
        }
    }

    protected virtual void ApplyDamageToEnemy(Enemy enemy)
    {
        enemy.TakeDamage(damage);
    }

    protected virtual void CreateBeam()
    {
        if (beamEffectPrefab != null)
        {
            currentBeam = Instantiate(beamEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    protected virtual void DestroyBeam()
    {
        if (currentBeam != null)
        {
            Destroy(currentBeam);
            currentBeam = null;
        }
    }

    protected virtual void StopAttack()
    {
        DestroyBeam();
        fireTimer = 0f;
    }
}