using UnityEngine;
using System.Collections;

public class SunBullet : Bullet
{
    [Header("子弹属性")]
    private float maxRadius;
    private float expandDuration;  // 扩张持续时间
    private LayerMask enemyLayer;

    private SpriteRenderer spriteRenderer;
    private float timer;
    private bool hasDealtDamage;  // 是否已经造成过伤害

    /// <summary>
    /// 初始化子弹
    /// </summary>
    public void Init(int damage, float maxRadius, float expandDuration, GameObject effect, LayerMask enemyLayer)
    {
        this.damage = damage;
        this.maxRadius = maxRadius;
        this.expandDuration = expandDuration;
        this.hitEffect = effect;
        this.enemyLayer = enemyLayer;

        spriteRenderer = GetComponent<SpriteRenderer>();

        // 初始大小为0
        transform.localScale = Vector3.zero;

        timer = 0f;
        hasDealtDamage = false;

        // 子弹在 expandDuration 秒后自动销毁
        Destroy(gameObject, expandDuration + 0.1f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // ========== 步骤1：子弹逐渐变大 ==========
        float progress = timer / expandDuration;  // 0 → 1
        float currentRadius = Mathf.Lerp(0, maxRadius, progress);

        // 设置大小（直径 = 半径 * 2）
        transform.localScale = new Vector3(currentRadius * 2, currentRadius * 2, 1);

        // ========== 步骤2：检测是否到达最大半径 ==========
        if (progress >= 1f && !hasDealtDamage)
        {
            // 造成伤害
            DealDamage();
            hasDealtDamage = true;
        }
    }

    /// <summary>
    /// 对范围内敌人造成伤害
    /// </summary>
    private void DealDamage()
    {
        // 检测范围内的所有敌人
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, maxRadius, enemyLayer);

        int hitCount = 0;
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    hitCount++;
                }
            }
        }

        Debug.Log($"范围攻击命中 {hitCount} 个敌人，伤害 {damage}");

        // 播放特效
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// 在编辑器中预览范围
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }
}