using UnityEngine;

public abstract class ContinuousAttackBullet : Bullet
{
    protected float tickInterval;
    protected float damagePerTick;
    protected Transform target;

    private float tickTimer;
    private LineRenderer lineRenderer;
    private float maxRange;

    [Header("连线设置")]
    [SerializeField] private int lineSortingOrder = 15;

    public void SetContinuousParams(Transform target, float maxRange, float tickInterval, int damagePerTick)
    {
        this.target = target;
        this.maxRange = maxRange;
        this.tickInterval = tickInterval;
        this.damagePerTick = damagePerTick;

        tickTimer = 0f;
        damage = damagePerTick;

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.sortingOrder = lineSortingOrder;
    }

    protected virtual void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance > maxRange)
        {
            Destroy(gameObject);
            return;
        }

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, target.position);
        }

        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                DealDamageToEnemy(enemy);
                ApplyEffect(enemy);
            }
        }
    }

    protected virtual void ApplyEffect(Enemy enemy) { }
}