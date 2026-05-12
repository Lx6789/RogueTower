using UnityEngine;

public class ClickBullet : ContinuousAttackBullet
{
    [Header("电击特效")]
    [SerializeField] private Color beamColor = new Color(0.2f, 0.5f, 1f, 1f);
    [SerializeField] private float beamWidth = 0.15f;
    [SerializeField] private float chainRange = 2f;
    [SerializeField] private int maxChainTargets = 2;
    [SerializeField] private float chainDamageMultiplier = 0.5f;

    public void SetClickParams(Color color, float width, float chainRange, int maxChains, float chainMultiplier)
    {
        beamColor = color;
        beamWidth = width;
        this.chainRange = chainRange;
        maxChainTargets = maxChains;
        chainDamageMultiplier = chainMultiplier;

        LineRenderer lr = GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.startColor = beamColor;
            lr.endColor = beamColor * 0.5f;
            lr.startWidth = beamWidth;
            lr.endWidth = beamWidth * 0.5f;
        }
    }
}