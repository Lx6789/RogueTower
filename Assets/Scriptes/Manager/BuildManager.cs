using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    [Header("瓦片地图")]
    [SerializeField] private Tilemap prefabTilemap;

    [Header("选择框")]
    [SerializeField] private GameObject selectionIndicator;

    [Header("建造面板")]
    [SerializeField] private GameObject buildPanelPrefab;
    [SerializeField] private Vector2 panelOffset = new Vector2(0, 80);

    private GameObject currentIndicator;
    private GameObject currentBuildPanel;
    private BuildPanelUI buildPanelUI;
    private Vector3Int currentSelectedCell;
    private int currentTowerIndex = 0;

    [Header("面板尺寸设置")]
    [SerializeField] private float cardWidth = 80f;
    [SerializeField] private float cardHeight = 120f;
    [SerializeField] private float cardSpacing = 10f;
    [SerializeField] private float panelPadding = 20f;

    private void Start()
    {
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }

        HideGroundTilemapRender();
        CreateBuildPanel();
    }

    private void Update()
    {
        HandleMouseClick();
        HandleTowerSwitch();
        HandleBuildInput();  // ? 添加建造输入处理
    }

    // ==================== 初始化 ====================

    private void CreateBuildPanel()
    {
        if (buildPanelPrefab != null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                currentBuildPanel = Instantiate(buildPanelPrefab, canvas.transform);
                buildPanelUI = currentBuildPanel.GetComponent<BuildPanelUI>();
                currentBuildPanel.SetActive(false);

                InitializePanelWithTowers();
            }
        }
    }

    private void InitializePanelWithTowers()
    {
        TowerData[] availableTowers = GetAvailableTowers();
        if (availableTowers == null || availableTowers.Length == 0) return;

        buildPanelUI.CreateTowerCards(availableTowers);
        AdjustPanelSize();
    }

    // ==================== 面板大小 ====================

    public void AdjustPanelSize()
    {
        if (currentBuildPanel == null || buildPanelUI == null) return;

        TowerData[] availableTowers = GetAvailableTowers();
        if (availableTowers == null || availableTowers.Length == 0) return;

        int towerCount = availableTowers.Length;

        float panelWidth = (cardWidth * towerCount) + (cardSpacing * (towerCount - 1)) + (panelPadding * 2);
        float panelHeight = cardHeight + (panelPadding * 2);

        RectTransform panelRect = currentBuildPanel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.sizeDelta = new Vector2(panelWidth, panelHeight);
        }
    }

    // ==================== 数据获取 ====================

    private TowerData[] GetAvailableTowers()
    {
        if (GameManager.Instance == null) return null;
        LevelConfig currentLevel = GameManager.Instance.GetCurrentLevelConfig();
        return currentLevel?.availableTowers;
    }

    // ==================== 地面处理 ====================

    private void HideGroundTilemapRender()
    {
        if (prefabTilemap == null) return;
        TilemapRenderer renderer = prefabTilemap.GetComponent<TilemapRenderer>();
        if (renderer != null) renderer.enabled = false;
    }

    // ==================== 输入处理 ====================

    private void HandleMouseClick()
    {
        // 检查鼠标是否在 UI 上，如果是则不处理瓦片点击
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            Vector3Int cellPosition = prefabTilemap.WorldToCell(mouseWorldPos);
            TileBase clickedTile = prefabTilemap.GetTile(cellPosition);

            if (clickedTile != null)
            {
                SelectCell(cellPosition);
            }
            else
            {
                DeselectCell();
            }
        }
    }

    private void HandleTowerSwitch()
    {
        if (currentBuildPanel == null || !currentBuildPanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            PreviousTower();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            NextTower();
        }
    }

    /// <summary>
    /// 处理建造输入（按空格键或点击卡片建造）
    /// </summary>
    private void HandleBuildInput()
    {
        if (currentBuildPanel == null || !currentBuildPanel.activeSelf) return;

        // 按空格键或回车键建造
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            BuildSelectedTower();
        }
    }

    // ==================== 选择逻辑 ====================

    private void SelectCell(Vector3Int cellPosition)
    {
        currentSelectedCell = cellPosition;
        ShowSelectionIndicator(cellPosition);
        ShowBuildPanel(cellPosition);
    }

    private void DeselectCell()
    {
        HideSelectionIndicator();
        HideBuildPanel();
    }

    // ==================== 选择框 ====================

    private void ShowSelectionIndicator(Vector3Int cellPosition)
    {
        Vector3 cellWorldPos = prefabTilemap.GetCellCenterWorld(cellPosition);

        if (currentIndicator == null)
        {
            currentIndicator = Instantiate(selectionIndicator, cellWorldPos, Quaternion.identity);
        }
        else
        {
            currentIndicator.transform.position = cellWorldPos;
        }
        currentIndicator.SetActive(true);
    }

    private void HideSelectionIndicator()
    {
        if (currentIndicator != null) currentIndicator.SetActive(false);
    }

    // ==================== 建造面板 ====================

    private void ShowBuildPanel(Vector3Int cellPosition)
    {
        if (currentBuildPanel == null || buildPanelUI == null) return;

        TowerData[] availableTowers = GetAvailableTowers();
        if (availableTowers == null || availableTowers.Length == 0) return;

        currentTowerIndex = 0;

        Vector3 cellWorldPos = prefabTilemap.GetCellCenterWorld(cellPosition);
        buildPanelUI.SetPosition(cellWorldPos, panelOffset);
        buildPanelUI.Show();
    }

    private void HideBuildPanel()
    {
        if (currentBuildPanel != null) currentBuildPanel.SetActive(false);
    }

    // ==================== 塔切换 ====================

    private void PreviousTower()
    {
        TowerData[] availableTowers = GetAvailableTowers();
        if (availableTowers == null || availableTowers.Length == 0) return;

        currentTowerIndex--;
        if (currentTowerIndex < 0) currentTowerIndex = availableTowers.Length - 1;
    }

    private void NextTower()
    {
        TowerData[] availableTowers = GetAvailableTowers();
        if (availableTowers == null || availableTowers.Length == 0) return;

        currentTowerIndex++;
        if (currentTowerIndex >= availableTowers.Length) currentTowerIndex = 0;
    }

    // ==================== 建造逻辑 ====================

    /// <summary>
    /// 建造当前选中的塔
    /// </summary>
    public void BuildSelectedTower()
    {
        TowerData selectedTower = GetCurrentSelectedTower();
        if (selectedTower == null)
        {
            Debug.LogError("没有选中塔！");
            return;
        }

        if (selectedTower.towerPrefab == null)
        {
            Debug.LogError($"塔 {selectedTower.towerName} 没有设置预制体！");
            return;
        }

        // 获取瓦片世界坐标
        Vector3 buildPosition = prefabTilemap.GetCellCenterWorld(currentSelectedCell);

        // 生成塔
        BuildTower(selectedTower, buildPosition);

        // 移除瓦片（该位置不可再建造）
        prefabTilemap.SetTile(currentSelectedCell, null);

        // 隐藏面板和选择框
        DeselectCell();
    }

    /// <summary>
    /// 生成塔
    /// </summary>
    private void BuildTower(TowerData towerData, Vector3 position)
    {
        GameObject towerObj = Instantiate(towerData.towerPrefab, position, Quaternion.identity);
        Tower tower = towerObj.GetComponent<Tower>();
        if (tower != null)
        {
            tower.Init(towerData);  // 塔的类型由预制体上的脚本决定
        }
    }

    /// <summary>
    /// 获取当前选中的塔数据
    /// </summary>
    public TowerData GetCurrentSelectedTower()
    {
        TowerData[] availableTowers = GetAvailableTowers();
        if (availableTowers == null || availableTowers.Length == 0) return null;
        if (currentTowerIndex >= availableTowers.Length) return null;
        return availableTowers[currentTowerIndex];
    }

    /// <summary>
    /// 通过索引建造塔（供卡片点击调用）
    /// </summary>
    public void BuildTowerByIndex(int index)
    {
        TowerData[] availableTowers = GetAvailableTowers();
        if (availableTowers == null || index >= availableTowers.Length) return;

        currentTowerIndex = index;
        BuildSelectedTower();
    }
}