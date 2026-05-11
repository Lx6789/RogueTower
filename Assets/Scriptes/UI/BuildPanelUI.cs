using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildPanelUI : MonoBehaviour
{
    [Header("UI 组件")]
    [SerializeField] private RectTransform panelRect;

    [Header("塔卡片模板")]
    [SerializeField] private GameObject towerCardPrefab;
    [SerializeField] private Transform cardsContainer;

    private List<GameObject> spawnedCards = new List<GameObject>();

    private void Awake()
    {
        if (panelRect == null)
        {
            panelRect = GetComponent<RectTransform>();
        }

        if (cardsContainer == null)
        {
            cardsContainer = transform;
        }
    }

    private void Start()
    {
        if (towerCardPrefab != null)
        {
            towerCardPrefab.SetActive(false);
        }
    }

    /// <summary>
    /// 根据可用塔数据创建所有卡片
    /// </summary>
    /// <param name="towers"></param>
    public void CreateTowerCards(TowerData[] towers)
    {
        ClearCards();

        if (towers == null || towers.Length == 0) return;
        if (towerCardPrefab == null) return;

        towerCardPrefab.SetActive(false);

        for (int i = 0; i < towers.Length; i++)  // ✅ 使用 for 循环传递索引
        {
            GameObject cardObj = Instantiate(towerCardPrefab, cardsContainer);
            cardObj.SetActive(true);

            TowerCardUI cardUI = cardObj.GetComponent<TowerCardUI>();
            if (cardUI == null)
            {
                cardUI = cardObj.AddComponent<TowerCardUI>();
            }

            cardUI.Setup(towers[i], i);  // ✅ 传递索引
            spawnedCards.Add(cardObj);
        }
    }

    /// <summary>
    /// 清除所有卡片
    /// </summary>
    private void ClearCards()
    {
        foreach (GameObject card in spawnedCards)
        {
            if (card != towerCardPrefab)
            {
                Destroy(card);
            }
        }
        spawnedCards.Clear();
    }

    /// <summary>
    /// 设置面板位置
    /// </summary>
    public void SetPosition(Vector3 worldPosition, Vector2 offset)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        screenPos += new Vector3(offset.x, offset.y, 0);
        transform.position = screenPos;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}