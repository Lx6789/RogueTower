using UnityEngine;

public abstract class ContinuousAttackTower : Tower
{
    protected GameObject currentBulletObj;   // 同一时间只会有一个子弹

    protected override void Shoot()
    {
        if (currentTarget == null || bulletPrefab == null) return;

        // 如果已有子弹，不重复生成（持续存在）
        if (currentBulletObj != null) return;

        currentBulletObj = ObjectPool.Instance.Get(bulletPrefab, shootPoint.position, Quaternion.identity);
        ContinuousAttackBullet bullet = currentBulletObj.GetComponent<ContinuousAttackBullet>();
        if (bullet != null)
        {
            InitBullet(bullet); 
        }
    }

    protected virtual void InitBullet(ContinuousAttackBullet bullet)
    {
        // 计算每次 tick 的伤害（damage 是总 DPS，tickInterval 是间隔）
        int tickDamage = Mathf.RoundToInt(damage * tickInterval);
        if (tickDamage <= 0) tickDamage = 1;

        bullet.Init(damage, 0, hitEffect, clip, bulletPrefab, enemyLayer, obstacleLayer);
        bullet.SetContinuousParams(currentTarget, range, tickInterval, tickDamage, transform);
    }

    protected override void OnUpdate()
    {
        if (currentBulletObj != null && currentTarget == null)
        {
            ObjectPool.Instance.Release(currentBulletObj, bulletPrefab);
            currentBulletObj = null;
        }
    }
}