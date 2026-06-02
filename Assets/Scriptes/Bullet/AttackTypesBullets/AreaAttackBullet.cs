using UnityEngine;
using System.Collections.Generic;

public abstract class AreaAttackBullet : Bullet
{
    [Header("范围攻击参数")]
    protected float maxRadius;
    protected float expandDuration;

    [Header("范围持续特效偏移")]
    [SerializeField] private Vector3 effectOffset = new Vector3(0, 0.5f, 0);

    private float timer;
    private HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

    // 为每个敌人创建的特效实例列表
    private List<GameObject> attachedEffects = new List<GameObject>();

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
        if (GameManager.IsPaused) return;
        if (expandDuration <= 0) return;

        timer += Time.deltaTime;
        float progress = timer / expandDuration;
        float currentRadius = Mathf.Lerp(0, maxRadius, progress);

        UpdateScaleFromRadius(currentRadius);
        CheckExpandingHit(currentRadius);

        if (progress >= 1f)
        {
            DealFinalDamage();
            Destroy(gameObject);
        }
    }

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

                // 为这个敌人附加特效
                AttachEffectToEnemy(enemy);
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
    }

    // 为敌人创建特效实例（使用基类的 hitEffect）
    private void AttachEffectToEnemy(Enemy enemy)
    {
        if (hitEffect == null || enemy == null) return;

        Vector3 spawnPos = enemy.transform.position + effectOffset;
        GameObject effect = Instantiate(hitEffect, spawnPos, hitEffect.transform.rotation);
        attachedEffects.Add(effect);

        // 获取 SFXPlayerViaManager 组件并初始化播放
        var sfx = effect.GetComponent<SFXPlayerViaManager>();
        if (sfx != null)
        {
            // 假设范围攻击的特效是循环的（如灼烧），传入 true
            sfx.InitAndPlay(clip, false);
        }

        // 粒子系统设置保持不变...
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.loop = true;
            if (!ps.isPlaying) ps.Play();
        }
    }

    // 销毁所有附加特效
    private void DestroyAllAttachedEffects()
    {
        foreach (var effect in attachedEffects)
        {
            if (effect != null)
            {
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                Destroy(effect);
            }
        }
        attachedEffects.Clear();
    }

    protected virtual void OnDestroy()
    {
        DestroyAllAttachedEffects();
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