using UnityEngine;
using System.Collections.Generic;

public abstract class AreaAttackBullet : Bullet
{
    protected float maxRadius;
    protected float expandDuration;

    private float timer;
    private HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

    public void SetAreaParams(float maxRadius, float expandDuration)
    {
        this.maxRadius = maxRadius;
        this.expandDuration = expandDuration;

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

        // 使用统一的缩放计算方法
        UpdateScaleFromRadius(currentRadius);

        CheckExpandingHit(currentRadius);

        if (progress >= 1f)
        {
            DealFinalDamage();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 根据半径更新 Sprite 缩放（与塔基类方法逻辑一致）
    /// </summary>
    protected virtual void UpdateScaleFromRadius(float radius)
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        float spriteSize = spriteRenderer.sprite.bounds.size.x;
        float targetScale = (radius * 2) / spriteSize;
        transform.localScale = new Vector3(targetScale, targetScale, 1);
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