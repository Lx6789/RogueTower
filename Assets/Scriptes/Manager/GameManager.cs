using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("获取关卡信息")]
    public LevelList LevelList;
    public UserData UserData;

    [Header("公用ui")]
    [Tooltip("面板")]
    [SerializeField] private Canvas canvas;

    [Header("赋值的开始界面的ui")]
    [SerializeField] private GameObject LevelPanel;
    [SerializeField] private GameObject SettingPanel;
    [SerializeField] private GameObject StartButton;
    [SerializeField] private GameObject ExitButton;
    [SerializeField] private GameObject SettingButton;
    [SerializeField] private GameObject ReturnButton;

    [Header("游戏界面的ui")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private GameObject TowerPanel;
    [SerializeField] private GameObject StopPanel;
    [SerializeField] private GameObject GameOverPanel;

    public Canvas mainCanvas;

    [Header("血条预制体")]
    public GameObject healthBarPrefab;

    [Header("游戏结束面板按钮")]
    [SerializeField] private GameObject nextLevelButton;  // 胜利时显示

    [Header("游戏物体")]
    public Transform baseTransform;

    [Header("游戏状态")]
    [SerializeField] private int currentLevelIndex = 0;

    public int currentScore = 0;
    public int currentGold;

    public static GameManager Instance { get; private set; }
    public int CurrentLevelIndex => currentLevelIndex;
    public bool IsGameOver => isGameOver;

    public static bool IsPaused { get; private set; } = false;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LevelList = Resources.Load<LevelList>("LevelList");
        UserData = Resources.Load<UserData>("UserData");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // 初始场景也由统一的 BGM 逻辑接管
        PlayBGMForScene(SceneManager.GetActiveScene().name);
        StartGameInit();
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RebindUI();
        ResetGameData();
        // 场景加载完，立刻切换合适的 BGM
        PlayBGMForScene(scene.name);
    }

    /// <summary>
    /// 根据场景名播放对应的背景音乐
    /// </summary>
    private void PlayBGMForScene(string sceneName)
    {
        if (MusicManager.Instance == null) return;

        if (sceneName == "MainScene")
        {
            MusicManager.Instance.PlayMainBGM();
        }
        else if (sceneName.StartsWith("Level_"))
        {
            MusicManager.Instance.PlayGameBGM();
        }
    }

    /// <summary>
    /// 重新绑定ui
    /// </summary>
    private void RebindUI()
    {
        var canvasGO = GameObject.Find("Canvas");
        if (canvasGO == null)
        {
            Debug.LogError("未找到Canvas！");
            return;
        }
        canvas = canvasGO.GetComponent<Canvas>();

        // 查找组件
        baseTransform = GameObject.Find("Base")?.transform;
        scoreText = GameObject.Find("ScoreTextUI")?.GetComponent<TMP_Text>();
        goldText = GameObject.Find("GoldTextUI")?.GetComponent<TMP_Text>();
        gameOverScoreText = GameObject.Find("GameOverScoreText")?.GetComponent<TMP_Text>();
        LevelPanel = GameObject.Find("LevelPanel");
        SettingPanel = GameObject.Find("SettingPanel");
        StartButton = GameObject.Find("StartButton");
        ExitButton = GameObject.Find("ExitButton");
        SettingButton = GameObject.Find("SettingButton");
        ReturnButton = GameObject.Find("ReturnButton");
        TowerPanel = GameObject.Find("TowerPanel");
        StopPanel = GameObject.Find("StopPanel");
        GameOverPanel = GameObject.Find("GameOverPanel");
        nextLevelButton = GameObject.Find("NextLevelButton");
        mainCanvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();

        // 初始隐藏面板
        if (TowerPanel) TowerPanel.SetActive(false);
        if (StopPanel) StopPanel.SetActive(false);
        if (GameOverPanel) GameOverPanel.SetActive(false);
        if (LevelPanel) LevelPanel.SetActive(false);
        if (SettingPanel) SettingPanel.SetActive(false);

        // ===== 动态绑定按钮 =====

        // 开始界面独立按钮
        BindButton(StartButton, () => GameManager.Instance.onStartButton());
        BindButton(ExitButton, () => GameManager.Instance.onExitButton());
        BindButton(SettingButton, () => GameManager.Instance.onSettingButton());
        if (LevelPanel != null)
        {
            BindButtonInChildren(LevelPanel, "ReturnButton", () => GameManager.Instance.onReturnButton());
        }
        else
        {
            Debug.LogWarning("LevelPanel 未找到");
        }
        if (SettingPanel != null)
        {
            BindButtonInChildren(SettingPanel, "SettingReturnButton", () => GameManager.Instance.onReturnButton());
        }
        else
        {
            Debug.LogWarning("SettingReturnButton 未找到");
        }

        // 游戏中独立按钮（直接在 Canvas 下）
        BindButton(GameObject.Find("PauseButton"), () => GameManager.Instance.onPauseButton());

        // 暂停面板内的按钮
        if (StopPanel != null)
        {
            BindButtonInChildren(StopPanel, "ContinueButton", () => GameManager.Instance.onContinueButton());
            BindButtonInChildren(StopPanel, "PauseRestartButton", () => GameManager.Instance.OnRestartButton());
            BindButtonInChildren(StopPanel, "PauseHomeButton", () => GameManager.Instance.OnHomeButton());
        }
        else
        {
            Debug.LogWarning("StopPanel 未找到");
        }

        // 游戏结束面板内的按钮
        if (GameOverPanel != null)
        {
            BindButtonInChildren(GameOverPanel, "GameOverRestartButton", () => GameManager.Instance.OnRestartButton());
            BindButtonInChildren(GameOverPanel, "GameOverHomeButton", () => GameManager.Instance.OnHomeButton());
            BindButtonInChildren(GameOverPanel, "NextLevelButton", () => GameManager.Instance.OnNextLevelButton());
        }
        else
        {
            Debug.LogWarning("GameOverPanel 未找到");
        }
    }

    private void BindButtonInChildren(GameObject parent, string buttonName, UnityEngine.Events.UnityAction action)
    {
        if (parent == null) return;

        Button[] allButtons = parent.GetComponentsInChildren<Button>(true);
        foreach (var btn in allButtons)
        {
            if (btn.gameObject.name == buttonName)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(action);
                Debug.Log($"成功绑定按钮: {buttonName}");
                return;
            }
        }
        Debug.LogError($"未在 {parent.name} 下找到按钮: {buttonName}");
    }

    private void BindButton(GameObject buttonObj, UnityEngine.Events.UnityAction action)
    {
        if (buttonObj == null)
        {
            Debug.LogWarning("BindButton: buttonObj 为 null");
            return;
        }
        Button btn = buttonObj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
            Debug.Log($"成功绑定按钮: {buttonObj.name}");
        }
        else
        {
            Debug.LogError($"物体 {buttonObj.name} 上没有 Button 组件！");
        }
    }

    /// <summary>
    /// 重新设置游戏数据
    /// </summary>
    public void ResetGameData()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        isGameOver = false;

        var levelCfg = GetCurrentLevelConfig();
        if (levelCfg != null)
        {
            currentGold = levelCfg.initialGold;
        }
        else
        {
            currentGold = 0;
        }
        currentScore = 0;

        if (goldText) goldText.text = "Gold: " + currentGold;
        if (scoreText) scoreText.text = "Score: " + currentScore;
    }

    // ==================== UI 按钮方法 ====================

    public void onStartButton()
    {
        if (StartButton) StartButton.SetActive(false);
        if (ExitButton) ExitButton.SetActive(false);
        if (SettingButton) SettingButton.SetActive(false);
        if (LevelPanel) LevelPanel.SetActive(true);
        if (SettingPanel) SettingPanel.SetActive(false);
    }

    public void onReturnButton()
    {
        if (StartButton) StartButton.SetActive(true);
        if (ExitButton) ExitButton.SetActive(true);
        if (SettingButton) SettingButton.SetActive(true);
        if (LevelPanel) LevelPanel.SetActive(false);
        if (SettingPanel) SettingPanel.SetActive(false);
    }

    public void onSettingButton()
    {
        if (StartButton) StartButton.SetActive(false);
        if (ExitButton) ExitButton.SetActive(false);
        if (SettingButton) SettingButton.SetActive(false);
        if (LevelPanel) LevelPanel.SetActive(false);
        if (SettingPanel) SettingPanel.SetActive(true);
    }

    public void onExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void onPauseButton()
    {
        if (isGameOver || IsPaused) return;
        Time.timeScale = 0f;
        IsPaused = true;
        closeTowerPanel();
        if (StopPanel) StopPanel.SetActive(true);
    }

    public void onContinueButton()
    {
        if (isGameOver) return;
        Time.timeScale = 1f;
        IsPaused = false;
        if (StopPanel) StopPanel.SetActive(false);
    }

    public void OnHomeButton()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene("MainScene");
    }

    public void OnRestartButton()
    {
        Debug.Log("OnRestartButton 被调用！");
        ResetGameData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnNextLevelButton()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= LevelList.levels.Length)
        {
            currentLevelIndex = 0;                // 重置为0，避免主场景越界
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            SceneManager.LoadScene("Level_" + (currentLevelIndex + 1));
        }
    }

    // ==================== 关卡数据 ====================

    public void LoadSelectLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        SceneManager.LoadScene("Level_" + (currentLevelIndex + 1));
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
        var levelCfg = GetCurrentLevelConfig();
        if (levelCfg != null)
        {
            currentGold = levelCfg.initialGold;
        }
        if (goldText) goldText.text = "Gold: " + currentGold;
        if (scoreText) scoreText.text = "Score: " + currentScore;
    }

    // ==================== 分数/金币 ====================

    public void updateGold(int gold)
    {
        currentGold += gold;
        if (goldText) goldText.text = "Gold: " + currentGold;
    }

    public void updateScord(int scord)
    {
        currentScore += scord;
        if (scoreText) scoreText.text = "Score: " + currentScore;
    }

    // ==================== 塔面板 ====================

    public void openTowerPanel(Vector3 worldPosition, GameObject tower)
    {
        if (isGameOver || IsPaused) return;

        RectTransform panelRect = TowerPanel.GetComponent<RectTransform>();
        Vector2 uiPos = WorldToCanvasPoint(worldPosition, canvas);
        panelRect.anchoredPosition = uiPos;
        panelRect.anchoredPosition += new Vector2(120, 50);

        TowerPanel.GetComponent<TowerPanel>().initTowerPanel(tower);
        TowerPanel.SetActive(true);
    }

    public void closeTowerPanel()
    {
        if (TowerPanel) TowerPanel.SetActive(false);
    }

    public void updatePanel(int level, int currentUpgradeCost, int currentSaleCost)
    {
        if (TowerPanel)
            TowerPanel.GetComponent<TowerPanel>().updatePanel(level, currentUpgradeCost, currentSaleCost);
    }

    // ==================== 游戏结束 ====================

    public void GameOver(int currentBaseHealth)
    {
        if (isGameOver) return;
        isGameOver = true;

        Time.timeScale = 0f;
        IsPaused = true;
        closeTowerPanel();
        if (StopPanel) StopPanel.SetActive(false);
        if (GameOverPanel) GameOverPanel.SetActive(true);

        gameOverScoreText.text = "SCORE: " + currentScore;

        bool isVictory = currentBaseHealth > 0;
        if (nextLevelButton != null)
            nextLevelButton.SetActive(isVictory);

        if (isVictory)
        {
            UserData.levelUserDatas[currentLevelIndex + 1].UnlockLevel();
            UserData.levelUserDatas[currentLevelIndex].setScore(currentScore);
        }
        else
        {
            UserData.levelUserDatas[currentLevelIndex].setScore(currentScore);
        }
    }

    // ==================== 工具方法 ====================

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