using UnityEngine;

public abstract class ContinuousAttackTower : Tower
{
    protected GameObject currentBulletObj;   // 同一时间只会有一个子弹

    protected override void Shoot()
    {
        if (currentTarget == null || bulletPrefab == null) return;

        // 如果已有子弹，不重复生成（持续存在）
        if (currentBulletObj != null) return;

        // 在射击点生成子弹
        currentBulletObj = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        ContinuousAttackBullet bullet = currentBulletObj.GetComponent<ContinuousAttackBullet>();
        if (bullet != null)
        {
            InitBullet(bullet);             // 调用子类可重写的初始化
        }
    }

    protected virtual void InitBullet(ContinuousAttackBullet bullet)
    {
        // 计算每次 tick 的伤害（damage 是总 DPS，tickInterval 是间隔）
        int tickDamage = Mathf.RoundToInt(damage * tickInterval);
        if (tickDamage <= 0) tickDamage = 1;

        bullet.Init(damage, 0, hitEffect, enemyLayer, obstacleLayer);
        bullet.SetContinuousParams(currentTarget, range, tickInterval, tickDamage, transform);
    }

    protected override void OnUpdate()
    {
        // 目标消失时销毁子弹
        if (currentBulletObj != null && currentTarget == null)
        {
            Destroy(currentBulletObj);
            currentBulletObj = null;
        }
    }
}