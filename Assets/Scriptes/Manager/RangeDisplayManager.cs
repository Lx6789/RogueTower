using UnityEngine;

public class RangeDisplayManager : MonoBehaviour
{
    private static Tower currentSelected;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 用 RaycastAll 找到鼠标位置所有碰撞体
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

            Tower tower = null;

            foreach (RaycastHit2D hit in hits)
            {
                // 跳过触发器（墙壁）
                if (hit.collider.isTrigger) continue;

                // 找 Tower 组件
                tower = hit.collider.GetComponent<Tower>();
                if (tower != null) break;
            }

            if (tower != null)
            {
                if (currentSelected != null && currentSelected != tower)
                    currentSelected.Deselect();
                currentSelected = tower;
            }
            else
            {
                ClearSelection();
            }
        }
    }

    private void ClearSelection()
    {
        if (currentSelected != null)
        {
            currentSelected.Deselect();
            currentSelected = null;
        }
    }
}