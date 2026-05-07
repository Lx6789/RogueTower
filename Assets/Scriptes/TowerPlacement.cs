using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    [Header("放置设置")]
    [Tooltip("PrefabMap")]
    [SerializeField] private Tilemap prefabMap;
    [Tooltip("塔的预制体")]
    [SerializeField] private GameObject towerPrefab;
    [Tooltip("造塔费用")]
    [SerializeField] private int towerCost;

    private void Start()
    {
        towerCost = towerPrefab.GetComponent<Tower>().Cost;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            Vector3Int cellPos = prefabMap.WorldToCell(mouseWorldPos);

            // 检查这个格子是否有可放置瓦片
            if (prefabMap.HasTile(cellPos))
            {
                // 钱够不够
                if (GameManager.Instance.SpendGold(towerCost))
                {
                    // 在格子中心生成塔
                    Vector3 towerPos = prefabMap.GetCellCenterWorld(cellPos);
                    towerPos.z = -1;
                    Instantiate(towerPrefab, towerPos, Quaternion.identity);

                    // 放完后移除瓦片，防止重复放置
                    prefabMap.SetTile(cellPos, null);
                }
                else
                {
                    Debug.Log("金币不足！");
                }
            }
        }
    }
}