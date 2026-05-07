using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    [Header("基础属性")]
    [Tooltip("炮塔名")]
    [SerializeField] protected string towerName = "默认塔";
    [Tooltip("攻击范围")]
    [SerializeField] protected float range = 3f;
    [Tooltip("攻击间隔")]
    [SerializeField] protected float fireRate = 0.5f;
    [Tooltip("伤害")]
    [SerializeField] protected int damage = 1;
    [Tooltip("炮塔等级")]
    [SerializeField] protected int level = 1;

    [Header("费用相关")]
    [Tooltip("建造费用")]
    [SerializeField] protected int cost = 50;
    [Tooltip("售卖金额")]
    [SerializeField] protected int salesAmount;
    [Tooltip("升级金额")]
    [SerializeField] protected int upgradeCost;

    [Header("子弹")]
    [SerializeField] protected GameObject bulletPrefab;

    [Header("升级")]
    [SerializeField] protected int maxLevel = 3;

    protected float fireTimer;
    protected Transform currentTarget;
    private bool isSelected = false;
    private GameObject rangeIndicator;

    public int UpgradeCost => upgradeCost;
    public int SellAmount => salesAmount;
    public int Cost => cost;

    private void Start()
    {
        upgradeCost = cost + level * 20;
        rangeIndicator = transform.Find("RangeIndicator")?.gameObject;
        if (rangeIndicator != null) rangeIndicator.SetActive(false);
    }

    private void Update()
    {
        FindTarget();
        HandleShooting();
    }

    /// <summary>
    /// 点击塔
    /// </summary>
    private void OnMouseDown()
    {
        isSelected = !isSelected;   // 点一下开，再点一下关
        Debug.Log(isSelected);
        UpdateRangeDisplay();

        if (isSelected)
        {
            TowerInfoPanel.Instance.Show(this);
        }
        else
        {
            TowerInfoPanel.Instance.Hide();
        }
    }

    /// <summary>
    /// 更新范围显示
    /// </summary>
    private void UpdateRangeDisplay()
    {
        if (rangeIndicator == null) return;

        if (isSelected)
        {
            rangeIndicator.SetActive(true);
            // 调整大小匹配 attackRange
            rangeIndicator.transform.localScale = new Vector3(range * 2, range * 2, 1);
        }
        else
        {
            rangeIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// 索敌
    /// </summary>
    protected void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
        //初始化"最近距离"为无穷大，用来找出离塔最近的敌人
        float closestDist = Mathf.Infinity;
        currentTarget = null;
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    currentTarget = hit.transform;
                }
            }
        }
    }

    /// <summary>
    /// 攻击
    /// </summary>
    protected abstract void Shoot();

    /// <summary>
    /// 攻击计时
    /// </summary>
    protected virtual void HandleShooting()
    {
        if (currentTarget == null) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            Shoot();
        }
    }

    /// <summary>
    /// 外部强制关闭范围显示
    /// </summary>
    public void Deselect()
    {
        isSelected = false;
        UpdateRangeDisplay();
    }

    /// <summary>
    /// 升级
    /// </summary>
    public virtual void Upgrade()
    {
        if (level >= maxLevel) return;

        if (GameManager.Instance.SpendGold(upgradeCost))
        {
            level++;
            damage *= level * 10;
            salesAmount = upgradeCost - level * 10;
            range += 0.5f;
            fireRate -= level * 0.2f;
            upgradeCost += level * 20;

            // 范围显示跟着变
            if (isSelected)
                UpdateRangeDisplay();

            Debug.Log($"{towerName} 升级到 {level} 级");
        }
    }

    /// <summary>
    /// 出售
    /// </summary>
    public virtual void Sell()
    {
        GameManager.Instance.AddGold(salesAmount);
        Destroy(gameObject);
    }
}
