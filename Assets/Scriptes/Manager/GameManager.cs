using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("获取关卡信息")]
    public LevelList LevelList;

    [Header("公用ui")]
    [Tooltip("面板")]
    [SerializeField] private Canvas canvas;

    [Header("赋值的开始界面的ui")]
    [Tooltip("选关面板")]
    [SerializeField] private GameObject LevelPanel;
    [Tooltip("开始按钮")]
    [SerializeField] private GameObject StartButton;
    [Tooltip("退出按钮")]
    [SerializeField] private GameObject ExitButton;
    [Tooltip("返回主界面按钮")]
    [SerializeField] private GameObject ReturnButton;

    [Header("游戏界面的ui")]
    [Tooltip("分数显示文本ui")]
    [SerializeField] private TMP_Text scoreText;
    [Tooltip("金币显示文本ui")]
    [SerializeField] private TMP_Text goldText;
    [Tooltip("塔的ui")]
    [SerializeField] private GameObject TowerPanel;

    [Header("游戏物体")]
    [Tooltip("基地位置")]
    public Transform baseTransform;

    [Header("游戏状态")]
    [SerializeField] private int currentLevelIndex = 0;  // 当前关卡索引

    public int currentScore = 0;
    public int currentGold;

    public static GameManager Instance { get; private set; }

    public int CurrentLevelIndex => currentLevelIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LevelList = Resources.Load<LevelList>("LevelList");
    }

    private void Start()
    {
        StartGameInit();
    }

    public void onStartButton()
    {
        StartButton.SetActive(false);
        ExitButton.SetActive(false);
        LevelPanel.SetActive(true);
    }

    public void onReturnButton()
    {
        StartButton.SetActive(true);
        ExitButton.SetActive(true);
        LevelPanel.SetActive(false);
    }

    public void onExitButton()
    {
        Debug.Log("退出游戏");
    }

    public LevelConfig GetCurrentLevelConfig()
    {
        if (LevelList == null || LevelList.levels == null ||
            currentLevelIndex >= LevelList.levels.Length)
        {
            Debug.LogError("无法获取当前关卡配置！");
            return null;
        }
        return LevelList.levels[currentLevelIndex];
    }

    public void SetCurrentLevel(int levelIndex)
    {
        if (LevelList != null && levelIndex < LevelList.levels.Length)
        {
            currentLevelIndex = levelIndex;
        }
    }

    private void StartGameInit()
    {
        currentGold = LevelList.levels[currentLevelIndex].initialGold;
        goldText.text = "Gold: " + currentGold;
        scoreText.text = "Score: " + currentScore;
    }

    public void updateGold(int gold)
    {
        currentGold += gold;
        goldText.text = "Gold: " + currentGold;
    }

    public void updateScord(int scord)
    {
        currentScore += scord;
        scoreText.text = "Score: " + currentScore;
    }

    /// <summary>
    /// 打开塔的信息面板，将面板放置在塔旁边（世界坐标 → UI坐标）
    /// </summary>
    /// <param name="worldPosition">塔的世界坐标</param>
    /// <param name="tower">塔的GameObject</param>
    public void openTowerPanel(Vector3 worldPosition, GameObject tower)
    {
        // 获取 TowerPanel 的 RectTransform
        RectTransform panelRect = TowerPanel.GetComponent<RectTransform>();

        // 世界坐标 => Canvas 局部坐标
        Vector2 uiPos = WorldToCanvasPoint(worldPosition, canvas);

        // 设置 anchoredPosition（局部坐标），而不是 position
        panelRect.anchoredPosition = uiPos;

        // 微调偏移，让面板不遮挡塔
         panelRect.anchoredPosition += new Vector2(120, 50);

        // 把塔的引用传给面板（用于升级/出售）
        TowerPanel.GetComponent<TowerPanel>().initTowerPanel(tower);

        // 显示面板
        TowerPanel.SetActive(true);
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    public void closeTowerPanel()
    {
        TowerPanel.SetActive(false);
    }

    /// <summary>
    /// 将世界坐标转换为 Canvas 上的 anchoredPosition
    /// </summary>
    public static Vector2 WorldToCanvasPoint(Vector3 worldPos, Canvas canvas, Camera camera = null)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
            camera ?? Camera.main,
            worldPos
        );

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        return localPoint;
    }
}