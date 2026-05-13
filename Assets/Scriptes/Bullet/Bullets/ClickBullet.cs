using UnityEngine;

public class ClickBullet : ContinuousAttackBullet
{
    public void ApplyVisualSettings(Color color, float width)
    {
        lineWidth = width;
        if (spriteRenderer != null)
            spriteRenderer.color = color;
    }
}