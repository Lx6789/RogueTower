using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("获取关卡信息")]
    public LevelList LevelList;

    [Header("赋值的开始界面的ui")]
    [Tooltip("选关面板")]
    [SerializeField] private GameObject LevelPanel;
    [Tooltip("开始按钮")]
    [SerializeField] private GameObject StartButton;
    [Tooltip("退出按钮")]
    [SerializeField] private GameObject ExitButton;
    [Tooltip("返回主界面按钮")]
    [SerializeField] private GameObject ReturnButton;

    [Header("游戏物体")]
    [Tooltip("基地位置")]
    public Transform baseTransform;

    [Header("游戏状态")]
    [SerializeField] private int currentLevelIndex = 0;  // 当前关卡索引

    public static GameManager Instance { get; private set; }

    public int CurrentLevelIndex => currentLevelIndex;

    private void Awake()
    {
        // 单例模式实现
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

    // 获取当前关卡配置
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

    // 设置当前关卡
    public void SetCurrentLevel(int levelIndex)
    {
        if (LevelList != null && levelIndex < LevelList.levels.Length)
        {
            currentLevelIndex = levelIndex;
        }
    }
}
