using UnityEngine;

public class SunTower : AreaAttackTower
{
    // 太阳塔可以有额外效果
    [Header("太阳塔专属")]
    [SerializeField] private float burnDuration = 2f;  // 灼烧持续时间

    // 如果有专属子弹，重写 Shoot
    // 否则使用基类的默认行为
}