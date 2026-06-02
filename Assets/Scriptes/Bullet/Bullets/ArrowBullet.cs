using UnityEngine;

public class ArrowBullet : FiredBullet
{
    [Header("击退参数")]
    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private System.Collections.IEnumerator StopKnockback(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(knockbackDuration);
        if (rb != null) rb.velocity = Vector2.zero;
    }
}