using UnityEngine;

public abstract class FiredBulletTower : Tower
{
    protected override void OnStart()
    {
        if (shootPoint == null) shootPoint = transform;
        if (obstacleLayer == 0) obstacleLayer = LayerMask.GetMask("ObstacleLayer");
    }

    protected override void Shoot()
    {
        if (currentTarget == null || bulletPrefab == null) return;

        GameObject bulletObj = ObjectPool.Instance.Get(bulletPrefab, shootPoint.position, Quaternion.identity);
        FiredBullet bullet = bulletObj.GetComponent<FiredBullet>();
        if (bullet != null)
            InitBullet(bullet);
    }

    protected virtual void InitBullet(FiredBullet bullet)
    {
        bullet.Init(damage, bulletSpeed, hitEffect, clip, bulletPrefab, enemyLayer, obstacleLayer);
        bullet.SetTarget(currentTarget);
    }
}