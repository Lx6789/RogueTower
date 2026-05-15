using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerPanel : MonoBehaviour
{
    [Tooltip("升级显示文本ui")]
    [SerializeField] private TMP_Text upgradeText;
    [Tooltip("售卖显示文本ui")]
    [SerializeField] private TMP_Text saleText;

    private GameObject Tower;

    /// <summary>
    /// 初始化塔面板
    /// </summary>
    /// <param name="tower"></param>
    public void initTowerPanel(GameObject tower)
    {
        Tower towerComp = tower?.GetComponent<Tower>();
        if (towerComp == null)
        {
            Debug.LogError("[TowerPanel] 塔物体上未找到 Tower 组件", tower);
            return;
        }
        Tower = tower;

        // 检查文本组件是否存在
        if (upgradeText == null || saleText == null)
        {
            Debug.LogError("[TowerPanel] 升级/出售文本未在 Inspector 中赋值！", this);
            return;
        }

        upgradeText.text = $"{towerComp.level}\nUpgrade\n{towerComp.currentUpgradeCost}";
        saleText.text = $"Sale\n{towerComp.currentSaleCost}";
    }

    public void OnUpgradeButton()
    {
        if (Tower == null) return;
        Tower.GetComponent<Tower>()?.Upgrade();
    }

    public void OnSaleButton()
    {
        if (Tower == null) return;
        Tower.GetComponent<Tower>()?.Sale();
    }

    public void updatePanel(int level, int currentUpgradeCost, int currentSaleCost)
    {
        upgradeText.text = $"{level}\nUpgrade\n{currentUpgradeCost}";
        saleText.text = $"Sale\n{currentSaleCost}";
    }
}