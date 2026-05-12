using UnityEngine;

public abstract class ContinuousAttackTower : Tower
{
    protected GameObject currentBulletObj;

    protected override void Shoot()
    {
        if (currentTarget == null || bulletPrefab == null) return;

        if (currentBulletObj == null)
        {
            currentBulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            ContinuousAttackBullet bullet = currentBulletObj.GetComponent<ContinuousAttackBullet>();
            if (bullet != null)
            {
                InitBullet(bullet);
            }
        }
    }

    protected virtual void InitBullet(ContinuousAttackBullet bullet)
    {
        // damage = 每秒伤害，每次tick伤害 = DPS × tickInterval
        int tickDamage = Mathf.RoundToInt(damage * tickInterval);
        if (tickDamage <= 0) tickDamage = 1;

        bullet.Init(damage, 0, hitEffect, enemyLayer, obstacleLayer);
        bullet.SetContinuousParams(currentTarget, range, tickInterval, tickDamage);
    }

    protected override void OnUpdate()
    {
        if (currentBulletObj != null && currentTarget == null)
        {
            Destroy(currentBulletObj);
            currentBulletObj = null;
        }
    }
}