using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainButtonManager : MonoBehaviour
{
    [Header("赋值的gameobject")]
    [Tooltip("选关面板")]
    public GameObject LevelPanel;
    [Tooltip("开始按钮")]
    public GameObject StartButton;
    [Tooltip("退出按钮")]
    public GameObject ExitButton;
    [Tooltip("返回主界面按钮")]
    public GameObject ReturnButton;

    public static MainButtonManager Instance { get; private set; }

    public void onStartButton()
    {
        StartButton.SetActive(false);
        ExitButton.SetActive(false);
        LevelPanel.SetActive(true);
    }

    public void onExitButton() 
    {
        Debug.Log("退出游戏");
    }

    public void onReturnButton()
    {
        StartButton.SetActive(true);
        ExitButton.SetActive(true);
        LevelPanel.SetActive(false);
    }
}
