using UnityEngine;

public class TowerSelector : MonoBehaviour
{
    public static TowerSelector Instance { get; private set; }

    private Tower selectedTower;
    private BuildManager buildManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        buildManager = FindObjectOfType<BuildManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null)
            {
                Tower tower = hit.GetComponent<Tower>();
                if (tower != null)
                {
                    SelectTower(tower);
                    return;
                }
            }

            if (buildManager == null || !buildManager.IsPanelActive())
            {
                DeselectTower();
            }
        }
    }

    public void SelectTower(Tower tower)
    {
        if (selectedTower != null && selectedTower != tower)
        {
            selectedTower.Deselect();
        }
        selectedTower = tower;
        tower.Select();
    }

    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.Deselect();
            selectedTower = null;
        }
    }
}