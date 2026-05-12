using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    private TowerData configData;
    private Collider2D towerCollider;

    [Header("塔属性（运行时）")]
    protected GameObject bulletPrefab;
    protected float fireRate;
    protected float range;
    protected int damage;
    protected float bulletSpeed;
    protected GameObject hitEffect;
    protected float expandDuration;
    protected float tickInterval;

    protected float fireTimer;
    protected Transform currentTarget;
    private bool isFirstShot = true;

    [Header("检测设置")]
    [SerializeField] private bool lockTarget = true;

    [Header("图层设置")]
    [SerializeField] protected LayerMask obstacleLayer;
    [SerializeField] protected LayerMask enemyLayer;

    [Header("范围指示器")]
    [SerializeField] protected GameObject rangeIndicatorPrefab;
    [SerializeField] protected Color rangeColor = new Color(1f, 1f, 1f, 0.2f);

    protected GameObject currentRangeIndicator;
    private bool isSelected = false;

    void Start()
    {
        if (enemyLayer == 0)
            enemyLayer = LayerMask.GetMask("Enemy");
        if (obstacleLayer == 0)
            obstacleLayer = LayerMask.GetMask("Wall");

        towerCollider = GetComponent<Collider2D>();
        if (towerCollider == null)
        {
            BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
            col.size = new Vector2(1, 1);
            towerCollider = col;
        }
        towerCollider.isTrigger = false;

        OnStart();
    }

    void Update()
    {
        FindTarget();
        HandleShooting();
        OnUpdate();
    }

    public virtual void Init(TowerData data)
    {
        configData = data;
        range = data.range;
        fireRate = data.attackInterval;
        bulletPrefab = data.bulletPrefab;
        damage = data.damage;
        hitEffect = data.BulletEffect;
        bulletSpeed = data.bulletSpeed;
        expandDuration = data.expandDuration;
        tickInterval = data.tickInterval;
    }

    protected virtual void OnStart() { }
    protected virtual void OnUpdate() { }
    protected abstract void Shoot();

    protected virtual void FindTarget()
    {
        if (lockTarget && currentTarget != null)
        {
            float distToTarget = Vector2.Distance(transform.position, currentTarget.position);
            if (distToTarget <= range && currentTarget.gameObject.activeInHierarchy)
                return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        float closestDist = Mathf.Infinity;
        Transform newTarget = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    newTarget = hit.transform;
                }
            }
        }

        currentTarget = newTarget;
    }

    protected virtual void HandleShooting()
    {
        if (currentTarget == null)
        {
            isFirstShot = true;
            return;
        }

        if (!currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;
            isFirstShot = true;
            return;
        }

        if (isFirstShot)
        {
            fireTimer = 0f;
            Shoot();
            isFirstShot = false;
            return;
        }

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            Shoot();
        }
    }

    // ==================== 范围指示器 ====================

    public void Select()
    {
        isSelected = true;
        ShowRangeIndicator();
    }

    public void Deselect()
    {
        isSelected = false;
        HideRangeIndicator();
    }

    private void ShowRangeIndicator()
    {
        if (rangeIndicatorPrefab == null) return;

        if (currentRangeIndicator == null)
        {
            currentRangeIndicator = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity, transform);
            SpriteRenderer sr = currentRangeIndicator.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = rangeColor;
                UpdateRangeIndicatorSize();
            }
        }
        currentRangeIndicator.SetActive(true);
    }

    private void HideRangeIndicator()
    {
        if (currentRangeIndicator != null)
            currentRangeIndicator.SetActive(false);
    }

    private void UpdateRangeIndicatorSize()
    {
        if (currentRangeIndicator == null) return;

        SpriteRenderer sr = currentRangeIndicator.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            float spriteSize = sr.sprite.bounds.size.x;
            float targetScale = (range * 2) / spriteSize;
            currentRangeIndicator.transform.localScale = new Vector3(targetScale, targetScale, 1);
        }
    }

    private void OnMouseDown()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        TowerSelector.Instance?.SelectTower(this);
    }

    private void OnDestroy()
    {
        if (currentRangeIndicator != null)
            Destroy(currentRangeIndicator);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}