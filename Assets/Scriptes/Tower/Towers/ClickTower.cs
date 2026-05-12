using UnityEngine;

public class ClickTower : ContinuousAttackTower
{
    [Header("电击塔设置")]
    [SerializeField] private Color beamColor = new Color(0.2f, 0.5f, 1f, 1f);
    [SerializeField] private float beamWidth = 0.15f;
    [SerializeField] private float chainRange = 2f;
    [SerializeField] private int maxChainTargets = 2;
    [SerializeField] private float chainDamageMultiplier = 0.5f;

    protected override void InitBullet(ContinuousAttackBullet bullet)
    {
        base.InitBullet(bullet);

        if (bullet is ClickBullet clickBullet)
        {
            clickBullet.SetClickParams(beamColor, beamWidth, chainRange, maxChainTargets, chainDamageMultiplier);
        }
    }
}