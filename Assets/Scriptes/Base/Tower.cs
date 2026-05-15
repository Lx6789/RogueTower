using UnityEngine;
using UnityEngine.Tilemaps;  // 引入 Tilemap 命名空间

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
    public int level = 1;
    private int maxLevel;

    [Header("检测设置")]
    [SerializeField] private bool lockTarget = true;

    [Header("图层设置")]
    [SerializeField] protected LayerMask obstacleLayer;
    [SerializeField] protected LayerMask enemyLayer;

    [Header("射击点（未赋值则使用塔自身位置）")]
    [SerializeField] protected Transform shootPoint;

    [Header("范围指示器")]
    [SerializeField] protected Color rangeColor = new Color(1f, 1f, 1f, 0.25f);
    [SerializeField] private float rangeLineWidth = 0.1f;
    [SerializeField] private int rangeLineSortingOrder = 5;
    [SerializeField] private int rangeSegments = 64;

    private LineRenderer rangeLine;
    private static Tower currentlySelectedTower;
    public int currentUpgradeCost;
    public int currentSaleCost;

    [HideInInspector] public Tilemap belongingTilemap;
    [HideInInspector] public Vector3Int cellPosition;
    [HideInInspector] public TileBase originalTile;

    protected virtual void Start()
    {
        if (enemyLayer == 0)
            enemyLayer = LayerMask.GetMask("Enemy");
        if (obstacleLayer == 0)
            obstacleLayer = LayerMask.GetMask("ObstacleLayer");

        towerCollider = GetComponent<Collider2D>();
        if (towerCollider == null)
        {
            BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
            col.size = new Vector2(1, 1);
            towerCollider = col;
        }
        towerCollider.isTrigger = false;

        if (shootPoint == null)
            shootPoint = transform;

        OnStart();
    }

    protected virtual void Update()
    {
        if (GameManager.IsPaused) return;

        FindTarget();
        HandleShooting();
        OnUpdate();

        if (currentlySelectedTower == this)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (UnityEngine.EventSystems.EventSystem.current != null &&
                    UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    return;

                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(mousePos);

                if (hit == null || hit.GetComponent<Tower>() == null)
                {
                    Deselect();
                }
            }
        }
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
        maxLevel = data.maxLevel;
        currentSaleCost = data.cost - level * 5;
        currentUpgradeCost = data.cost + level * 10;
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
        if (GameManager.IsPaused) return;

        if (currentlySelectedTower != null && currentlySelectedTower != this)
            currentlySelectedTower.Deselect();

        currentlySelectedTower = this;
        GameManager.Instance.openTowerPanel(transform.position, gameObject);
        ShowRangeIndicator();
    }

    public void Deselect()
    {
        if (currentlySelectedTower == this)
            currentlySelectedTower = null;

        GameManager.Instance.closeTowerPanel();
        HideRangeIndicator();
    }

    private void ShowRangeIndicator()
    {
        if (rangeLine == null)
        {
            GameObject lineObj = new GameObject("RangeIndicator");
            lineObj.transform.SetParent(transform);
            lineObj.transform.localPosition = Vector3.zero;

            rangeLine = lineObj.AddComponent<LineRenderer>();
            rangeLine.useWorldSpace = false;
            rangeLine.loop = true;
            rangeLine.startWidth = rangeLineWidth;
            rangeLine.endWidth = rangeLineWidth;
            rangeLine.material = new Material(Shader.Find("Sprites/Default"));
            rangeLine.startColor = rangeColor;
            rangeLine.endColor = rangeColor;
            rangeLine.sortingOrder = rangeLineSortingOrder;
        }

        UpdateRangeIndicatorSize();
        rangeLine.enabled = true;
    }

    private void HideRangeIndicator()
    {
        if (rangeLine != null)
            rangeLine.enabled = false;
    }

    protected virtual void UpdateRangeIndicatorSize()
    {
        if (rangeLine == null) return;

        rangeLine.positionCount = rangeSegments + 1;
        float angleStep = 360f / rangeSegments;

        for (int i = 0; i <= rangeSegments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 point = new Vector3(Mathf.Cos(angle) * range, Mathf.Sin(angle) * range, 0);
            rangeLine.SetPosition(i, point);
        }
    }

    protected float CalculateScaleFromRadius(float radius, SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null) return 1f;
        float spriteSize = spriteRenderer.sprite.bounds.size.x;
        return (radius * 2) / spriteSize;
    }

    private void OnMouseDown()
    {
        if (GameManager.IsPaused) return;

        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        Select();
    }

    /// <summary>
    /// 销毁塔
    /// </summary>
    private void OnDestroy()
    {
        // 恢复瓦片
        if (belongingTilemap != null)
        {
            belongingTilemap.SetTile(cellPosition, originalTile);
        }

        if (rangeLine != null)
            Destroy(rangeLine.gameObject);
        if (currentlySelectedTower == this)
            currentlySelectedTower = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void Upgrade()
    {
        if (GameManager.IsPaused) return;

        if (level >= maxLevel || GameManager.Instance.currentGold < currentUpgradeCost) return;

        int costToDeduct = currentUpgradeCost;

        level++;
        damage += 10;
        range += 0.5f;
        tickInterval -= 0.5f;

        currentUpgradeCost = costToDeduct + level * 10;
        currentSaleCost = currentUpgradeCost - level * 5;

        GameManager.Instance.updateGold(-costToDeduct);
        GameManager.Instance.updatePanel(level, currentUpgradeCost, currentSaleCost);

        if (currentlySelectedTower == this && rangeLine != null)
        {
            UpdateRangeIndicatorSize();
        }
    }

    public void Sale()
    {
        if (GameManager.IsPaused) return;

        GameManager.Instance.updateGold(currentSaleCost);
        GameManager.Instance.closeTowerPanel();
        Destroy(gameObject);  // 这里会触发 OnDestroy，自动恢复瓦片
    }
}