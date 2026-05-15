using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    public static MainMenuManager Instance { get; private set; }

    /// <summary>
    /// 按下开始按钮
    /// </summary>
    public void onStartButton()
    {
        GameManager.Instance.onStartButton();
    }

    /// <summary>
    /// 按下退出按钮
    /// </summary>
    public void onExitButton() 
    {
        GameManager.Instance.onExitButton();
    }

    /// <summary>
    /// 按下返回按钮
    /// </summary>
    public void onReturnButton()
    {
        GameManager.Instance.onReturnButton();
    }
}
