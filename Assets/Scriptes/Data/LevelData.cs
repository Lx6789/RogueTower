using UnityEngine;

[CreateAssetMenu(fileName = "New LevelData", menuName = "关卡配置")]
public class LevelData : ScriptableObject
{
    [Header("关卡信息")]
    [Tooltip("关卡名")]
    public string levelName;
    [Tooltip("关卡描述")]
    [TextArea] public string description;
    [Tooltip("基地血量")]
    public int baseHealth = 10;
    [Tooltip("初始金币")]
    public int initialGold = 200;

    [Header("可用塔")]
    public TowerData[] availableTowers;

    [Header("波次列表")]
    public WaveData[] waves;

    [System.Serializable]
    public class WaveData 
    {
        [System.Serializable]
        public class EnemySpawnEntry
        {
            [Tooltip("敌人种类")]
            public EnemyData enemyData;
            [Tooltip("数量")]
            public int count;
            [Tooltip("生成间隔")]
            public float spawnInterval;
        }

        [Header("配置敌人数据")]
        public EnemySpawnEntry[] enemies;
        [Header("波次开始前等待时间")]
        public float delayBeforeWave = 3f;
    }
}