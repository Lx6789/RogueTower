using UnityEngine;

public class ArrowBullet : Bullet
{
    private float knockbackForce;
    private float knockbackDuration;

    /// <summary>
    /// 设置击退效果
    /// </summary>
    public void SetKnockback(float force, float duration)
    {
        knockbackForce = force;
        knockbackDuration = duration;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                // 击退效果
                ApplyKnockback(enemy);
            }

            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 击退敌人
    /// </summary>
    private void ApplyKnockback(Enemy enemy)
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            // 计算击退方向（子弹飞行方向的反方向）
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;

            // 应用击退
            enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

            // 可选：延迟停止击退
            StartCoroutine(StopKnockback(enemyRb));
        }
    }

    private System.Collections.IEnumerator StopKnockback(Rigidbody2D rb)
    {
        yield return new WaitForSeconds(knockbackDuration);
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
}