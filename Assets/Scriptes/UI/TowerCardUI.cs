using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TowerCardUI : MonoBehaviour
{
    [Header("卡片 UI 组件")]
    [SerializeField] private Image towerIcon;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button cardButton;  // ✅ 添加按钮组件

    private int towerIndex;  // 塔在列表中的索引

    private void Awake()
    {
        if (towerIcon == null)
        {
            Transform iconTransform = transform.Find("TowerIcon");
            if (iconTransform != null)
            {
                towerIcon = iconTransform.GetComponent<Image>();
            }
        }

        if (costText == null)
        {
            Transform costTransform = transform.Find("CostText");
            if (costTransform != null)
            {
                costText = costTransform.GetComponent<TextMeshProUGUI>();
            }
        }

        // ✅ 如果没有按钮组件，自动添加
        if (cardButton == null)
        {
            cardButton = GetComponent<Button>();
            if (cardButton == null)
            {
                cardButton = gameObject.AddComponent<Button>();
            }
        }

        // ✅ 设置按钮点击事件
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnCardClicked);
        }
    }

    /// <summary>
    /// 设置卡片数据
    /// </summary>
    public void Setup(TowerData towerData, int index)
    {
        towerIndex = index;

        if (towerIcon != null)
        {
            towerIcon.sprite = towerData.towerIcon;
        }

        if (costText != null)
        {
            costText.text = $"{towerData.cost}";
        }
    }

    /// <summary>
    /// 卡片点击事件
    /// </summary>
    private void OnCardClicked()
    {
        BuildManager buildManager = FindObjectOfType<BuildManager>();
        if (buildManager != null)
        {
            buildManager.BuildTowerByIndex(towerIndex);
        }
    }
}