using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [Header("子弹属性")]
    protected int damage;
    protected float speed;
    protected GameObject hitEffect;
    protected LayerMask targetLayer;
    protected LayerMask obstacleLayer;

    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;

    public virtual void Init(int damage, float speed, GameObject effect,
        LayerMask targetLayer = default, LayerMask obstacleLayer = default)
    {
        this.damage = damage;
        this.speed = speed;
        this.hitEffect = effect;
        this.targetLayer = targetLayer;
        this.obstacleLayer = obstacleLayer;
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void DealDamageToEnemy(Enemy enemy)
    {
        if (enemy != null)
            enemy.TakeDamage(damage);
    }

    protected void PlayHitEffect()
    {
        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);
    }

    protected bool IsEnemy(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return false;
        if (targetLayer != 0 && (targetLayer.value & (1 << collision.gameObject.layer)) == 0) return false;
        return true;
    }
}