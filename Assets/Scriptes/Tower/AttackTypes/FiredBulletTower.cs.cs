using UnityEngine;

public abstract class FiredBulletTower : Tower
{
    [Header("射击点")]
    [SerializeField] protected Transform shootPoint;

    protected override void OnStart()
    {
        if (shootPoint == null)
        {
            shootPoint = transform;
        }

        // 默认障碍物层为 Wall
        if (obstacleLayer == 0)
        {
            obstacleLayer = LayerMask.GetMask("Obstacle");
        }
    }

    protected override void Shoot()
    {
        if (currentTarget == null || bulletPrefab == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            InitBullet(bullet);
        }
        else
        {
            Debug.LogError("子弹预制体上没有 Bullet 组件！");
        }
    }

    /// <summary>
    /// 初始化子弹（统一传递图层参数）
    /// </summary>
    protected virtual void InitBullet(Bullet bullet)
    {
        bullet.Init(
            damage,           // 伤害
            bulletSpeed,      // 速度
            currentTarget,    // 目标
            hitEffect,        // 击中特效
            enemyLayer,       // 目标层：Enemy
            obstacleLayer     // 障碍物层：Obstacle
        );
    }
}