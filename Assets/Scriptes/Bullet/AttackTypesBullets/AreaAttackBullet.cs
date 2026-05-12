using UnityEngine;
using System.Collections.Generic;

public abstract class AreaAttackBullet : Bullet
{
    protected float maxRadius;
    protected float expandDuration;

    private float timer;
    private float spriteSize;
    private HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

    public void SetAreaParams(float maxRadius, float expandDuration)
    {
        this.maxRadius = maxRadius;
        this.expandDuration = expandDuration;

        // ✅ 自适应图片大小
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            spriteSize = spriteRenderer.sprite.bounds.size.x;
        }
        else
        {
            spriteSize = 1f;
        }

        transform.localScale = Vector3.zero;
        timer = 0f;
        hitEnemies.Clear();
    }

    protected virtual void Update()
    {
        if (expandDuration <= 0) return;

        timer += Time.deltaTime;
        float progress = timer / expandDuration;
        float currentRadius = Mathf.Lerp(0, maxRadius, progress);
        float targetScale = (currentRadius * 2) / spriteSize;
        transform.localScale = new Vector3(targetScale, targetScale, 1);

        CheckExpandingHit(currentRadius);

        if (progress >= 1f)
        {
            DealFinalDamage();
            Destroy(gameObject);
        }
    }

    private void CheckExpandingHit(float currentRadius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentRadius, targetLayer);

        foreach (Collider2D hit in hits)
        {
            if (!IsEnemy(hit)) continue;

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                DealDamageToEnemy(enemy);
                ApplyEffect(enemy);
                hitEnemies.Add(enemy);
            }
        }
    }

    private void DealFinalDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, maxRadius, targetLayer);

        foreach (Collider2D hit in hits)
        {
            if (!IsEnemy(hit)) continue;

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                DealDamageToEnemy(enemy);
                ApplyEffect(enemy);
                hitEnemies.Add(enemy);
            }
        }

        PlayHitEffect();
    }

    protected virtual void ApplyEffect(Enemy enemy) { }

    void OnDrawGizmosSelected()
    {
        if (maxRadius > 0)
        {
            Gizmos.color = new Color(1, 1, 0, 0.3f);
            Gizmos.DrawWireSphere(transform.position, maxRadius);
        }
    }
}