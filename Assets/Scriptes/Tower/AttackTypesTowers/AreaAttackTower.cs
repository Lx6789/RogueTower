using UnityEngine;

public abstract class AreaAttackTower : Tower
{
    protected override void Shoot()
    {
        if (currentTarget == null || bulletPrefab == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        AreaAttackBullet bullet = bulletObj.GetComponent<AreaAttackBullet>();
        if (bullet != null)
            InitBullet(bullet);
    }

    protected virtual void InitBullet(AreaAttackBullet bullet)
    {
        bullet.Init(damage, 0, hitEffect, enemyLayer);
        bullet.SetAreaParams(range, expandDuration);
    }
}