using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [Header("子弹属性")]
    protected int damage;
    protected float speed;
    protected GameObject hitEffect;
    protected LayerMask targetLayer;
    protected LayerMask obstacleLayer;
    protected AudioClip clip;

    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;

    public virtual void Init(int damage, float speed, GameObject effect, AudioClip clip,
        LayerMask targetLayer = default, LayerMask obstacleLayer = default)
    {
        this.damage = damage;
        this.speed = speed;
        this.hitEffect = effect;
        this.targetLayer = targetLayer;
        this.obstacleLayer = obstacleLayer;
        this.clip = clip;
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 对敌人造成伤害
    /// </summary>
    /// <param name="enemy"></param>
    protected void DealDamageToEnemy(Enemy enemy)
    {
        if (enemy != null)
            enemy.TakeDamage(damage);
    }

    /// <summary>
    /// 判断是否是敌人
    /// </summary>
    /// <param name="collision"></param>
    /// <returns></returns>
    protected bool IsEnemy(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return false;
        if (targetLayer != 0 && (targetLayer.value & (1 << collision.gameObject.layer)) == 0) return false;
        return true;
    }
}