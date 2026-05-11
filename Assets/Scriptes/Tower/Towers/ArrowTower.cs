using UnityEngine;

public class ArrowTower : FiredBulletTower
{
    [Header("箭塔专属效果")]
    [SerializeField] private float knockbackForce = 3f;  // 击退力度
    [SerializeField] private float knockbackDuration = 0.2f;  // 击退持续时间

    protected override void InitBullet(Bullet bullet)
    {
        // 调用基类初始化
        base.InitBullet(bullet);

        // 如果子弹是 ArrowBullet，设置击退效果
        ArrowBullet arrowBullet = bullet as ArrowBullet;
        if (arrowBullet != null)
        {
            arrowBullet.SetKnockback(knockbackForce, knockbackDuration);
        }
    }
}