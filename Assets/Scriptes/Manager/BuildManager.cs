using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildManager : MonoBehaviour
{
    [Header("建造设置")]
    public GameObject[] towerPrefabs;  // 塔的预制体数组
    public Tilemap prefabTilemap;
    public Tilemap obstacleTilemap;

    [Header("UI")]
    public GameObject buildPanel;      // 建造面板 UI

    private int selectedTowerIndex = 0;  // 当前选择的塔索引
    private bool isBuildMode = false;    // 是否在建造模式

    private void Update()
    {
        if (!isBuildMode) return;

        if (Input.GetMouseButtonDown(0))
        {
            TryBuildTower();
        }

        if (Input.GetMouseButtonDown(1))  // 右键取消
        {
            CancelBuild();
        }

        // 显示半透明的塔预览
        UpdateBuildPreview();
    }

    void TryBuildTower()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // 检查鼠标是否在 UI 上
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        // 获取瓦片坐标
        Vector3Int cellPos = prefabTilemap.WorldToCell(mouseWorldPos);

        // 检查该位置是否可以建造
        if (CanBuildAt(cellPos))
        {
            Vector3 buildPos = prefabTilemap.GetCellCenterWorld(cellPos);
            BuildTower(buildPos);
        }
        else
        {
            Debug.Log("此处不能建造！");
        }
    }

    bool CanBuildAt(Vector3Int cellPos)
    {
        // 1. 检查地面 Tilemap 是否有瓦片
        TileBase groundTile = prefabTilemap.GetTile(cellPos);
        if (groundTile == null)
            return false;

        // 2. 检查障碍物 Tilemap 是否有瓦片
        if (obstacleTilemap != null)
        {
            TileBase obstacleTile = obstacleTilemap.GetTile(cellPos);
            if (obstacleTile != null)
                return false;
        }

        // 3. 检查该位置是否已经有塔（使用 Physics2D 检测）
        Vector3 worldPos = prefabTilemap.GetCellCenterWorld(cellPos);
        Collider2D existingTower = Physics2D.OverlapCircle(worldPos, 0.3f, LayerMask.GetMask("Tower"));
        if (existingTower != null)
            return false;

        return true;
    }

    void BuildTower(Vector3 position)
    {
        if (selectedTowerIndex >= 0 && selectedTowerIndex < towerPrefabs.Length)
        {
            Instantiate(towerPrefabs[selectedTowerIndex], position, Quaternion.identity);

            // 可选：消耗金币
            // GameManager.Instance.SpendGold(towerCost);
        }
    }

    void CancelBuild()
    {
        isBuildMode = false;
        buildPanel.SetActive(false);
    }

    void UpdateBuildPreview()
    {
        // 显示建造预览（半透明塔跟随鼠标）
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector3Int cellPos = prefabTilemap.WorldToCell(mouseWorldPos);
        Vector3 snapPos = prefabTilemap.GetCellCenterWorld(cellPos);

        // 这里可以移动预览对象到 snapPos
    }

    // UI 按钮调用：选择塔
    public void SelectTower(int index)
    {
        selectedTowerIndex = index;
        isBuildMode = true;
    }
}
