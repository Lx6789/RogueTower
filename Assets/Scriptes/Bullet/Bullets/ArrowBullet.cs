using UnityEngine;

public class ArrowBullet : FiredBullet
{
    [Header("击退参数")]
    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float knockbackDuration = 0.2f;

    protected override void ApplyEffect(Enemy enemy)
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
            enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            StartCoroutine(StopKnockback(enemyRb));
        }
    }

    private System.Collections.IEnumerator StopKnockback(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(knockbackDuration);
        if (rb != null) rb.velocity = Vector2.zero;
    }
}