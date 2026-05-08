using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("配置信息")]
    [Tooltip("当前关卡配置信息")]
    [SerializeField] private LevelData currentLevelData;

    public static LevelData CurrentLevel => Instance.currentLevelData;


}
