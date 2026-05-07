using UnityEngine;
using UnityEngine.Tilemaps;

public class HighlightIndicator : MonoBehaviour
{
    [SerializeField] private Tilemap prefabMap;
    [SerializeField] private Sprite highlightSprite;

    private GameObject currentHighlight;
    private Vector3Int lastCell;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            Vector3Int cellPos = prefabMap.WorldToCell(mouseWorldPos);

            // 点击的是可建造格子
            if (prefabMap.HasTile(cellPos))
            {
                // 如果点的是同一个格子，关闭高亮
                if (cellPos == lastCell && currentHighlight != null)
                {
                    Destroy(currentHighlight);
                    lastCell = new Vector3Int(int.MinValue, int.MinValue, 0);
                    return;
                }

                // 销毁旧的高亮
                if (currentHighlight != null)
                    Destroy(currentHighlight);

                // 在格子中心生成高亮图片
                Vector3 pos = prefabMap.GetCellCenterWorld(cellPos);
                currentHighlight = new GameObject("Highlight");
                currentHighlight.transform.position = new Vector3(pos.x, pos.y, -0.5f);  // Z轴靠前
                SpriteRenderer sr = currentHighlight.AddComponent<SpriteRenderer>();
                sr.sprite = highlightSprite;
                sr.color = new Color(1f, 1f, 1f, 0.3f);  // 半透明白色
                sr.sortingOrder = 20;  // 确保显示在瓦片上面

                lastCell = cellPos;
            }
            else
            {
                // 点击不是可建造格子，清除高亮
                if (currentHighlight != null)
                {
                    Destroy(currentHighlight);
                    lastCell = new Vector3Int(int.MinValue, int.MinValue, 0);
                }
            }
        }
    }
}