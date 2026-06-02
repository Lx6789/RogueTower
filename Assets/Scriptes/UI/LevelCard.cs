using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class LevelCard : MonoBehaviour
{
    public GameObject LockUI;

    public int levelIndex;

    public bool isUnlock;

    /// <summary>
    /// 设置关卡
    /// </summary>
    /// <param name="index"></param>
    public void setLevelIndex(int index)
    {
        levelIndex = index;
    }

    /// <summary>
    /// 设置锁
    /// </summary>
    /// <param name="isLock"></param>
    public void setLevelLock(bool isUnlock)
    {
        LockUI.SetActive(!isUnlock);
        this.isUnlock = isUnlock;
    }

    /// <summary>
    /// 点击关卡
    /// </summary>
    public void onCLick()
    {
        if (isUnlock) GameManager.Instance.LoadSelectLevel(levelIndex);
    }
}
