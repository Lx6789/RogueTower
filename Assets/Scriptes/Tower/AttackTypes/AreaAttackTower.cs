using UnityEngine;

public abstract class AreaAttackTower : Tower
{
    protected override void Shoot()
    {
        if (currentTarget == null || bulletPrefab == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        SunBullet areaBullet = bulletObj.GetComponent<SunBullet>();
        if (areaBullet != null)
        {
            InitAreaBullet(areaBullet);
        }
        else
        {
            Debug.LogError("范围攻击子弹预制体需要挂载 AreaBullet 脚本！");
        }
    }

    /// <summary>
    /// 初始化范围子弹（统一传递参数）
    /// </summary>
    protected virtual void InitAreaBullet(SunBullet areaBullet)
    {
        areaBullet.Init(
            damage,         // 伤害
            range,          // 最大半径
            fireRate,       // 扩张时间 = 攻击间隔
            hitEffect,      // 特效
            enemyLayer      // 目标层：Enemy
        );
    }
}