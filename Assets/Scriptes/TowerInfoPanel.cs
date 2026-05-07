using UnityEngine;
using UnityEngine.UI;

public class TowerInfoPanel : MonoBehaviour
{
    public static TowerInfoPanel Instance { get; private set; }

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Text upgradeCostText;
    [SerializeField] private Text sellAmountText;

    private Tower currentTower;

    void Awake()
    {
        Instance = this;
        panelRoot.SetActive(false);

        upgradeButton.onClick.AddListener(OnUpgradeClick);
        sellButton.onClick.AddListener(OnSellClick);
    }

    /// <summary>
    /// 显示塔的攻击范围和按钮
    /// </summary>
    /// <param name="tower"></param>
    public void Show(Tower tower)
    {
        currentTower = tower;
        panelRoot.SetActive(true);
        UpdateUI();
        UpdatePosition();
    }

    /// <summary>
    /// 隐藏按钮
    /// </summary>
    public void Hide()
    {
        currentTower = null;
        panelRoot.SetActive(false);
    }

    /// <summary>
    /// 更新按钮
    /// </summary>
    void UpdateUI()
    {
        if (currentTower != null)
        {
            upgradeCostText.text = $"升级: {currentTower.UpgradeCost}G";
            sellAmountText.text = $"出售: {currentTower.SellAmount}G";
        }
    }

    /// <summary>
    /// 更新按钮ui位置
    /// </summary>
    void UpdatePosition()
    {
        if (currentTower == null) return;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(currentTower.transform.position);
        screenPos.y += 120f;
        screenPos.x -= 50f;
        transform.position = screenPos;
    }

    /// <summary>
    /// 点击升级按钮
    /// </summary>
    public void OnUpgradeClick()
    {
        currentTower?.Upgrade();
        UpdateUI();
    }

    /// <summary>
    /// 点击售卖按钮
    /// </summary>
    public void OnSellClick()
    {
        currentTower?.Sell();
        Hide();
    }
}