using UnityEngine;

[CreateAssetMenu(fileName = "LevelList", menuName = "关卡配置")]
public class LevelList : ScriptableObject
{
    public LevelConfig[] levels;
}

// 普通的数据类，不是 ScriptableObject
[System.Serializable]
public class LevelConfig
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
        public EnemySpawnEntry[] enemies;
        public float delayBeforeWave = 3f;

        [System.Serializable]
        public class EnemySpawnEntry
        {
            public EnemyData enemyData;
            public int count;
            public float spawnInterval;
        }
    }
}