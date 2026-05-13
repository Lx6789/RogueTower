using UnityEngine;

public class ClickTower : ContinuousAttackTower
{
    [SerializeField] private Color beamColor = new Color(0.2f, 0.5f, 1f, 1f);
    [SerializeField] private float beamWidth = 0.15f;

    protected override void InitBullet(ContinuousAttackBullet bullet)
    {
        base.InitBullet(bullet);
        if (bullet is ClickBullet clickBullet)
            clickBullet.ApplyVisualSettings(beamColor, beamWidth);
    }
}