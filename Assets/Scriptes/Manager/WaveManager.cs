using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("当前关卡配置")]
    private LevelList levelList;
    [Header("怪物出生点")]
    public Transform spawnPoint;

    private int currentLevelIndex = 0;  // 当前关卡索引
    private int currentWaveIndex = 0;   // 当前波次索引
    private int enemiesAlive = 0;       // 存活的敌人数量

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        levelList = GameManager.Instance.LevelList;
        // 开始第一关的所有波次
        StartCoroutine(ProcessLevel(currentLevelIndex));
    }

    /// <summary>
    /// 处理整个关卡的所有波次
    /// </summary>
    IEnumerator ProcessLevel(int levelIndex)
    {
        if (levelList == null || levelIndex >= levelList.levels.Length)
        {
            Debug.LogError("无效的关卡索引！");
            yield break;
        }

        LevelConfig currentLevel = levelList.levels[levelIndex];
        Debug.Log($"开始关卡: {currentLevel.levelName}");

        // 遍历所有波次
        for (int waveIndex = 0; waveIndex < currentLevel.waves.Length; waveIndex++)
        {
            Debug.Log($"准备第 {waveIndex + 1} 波");

            // 生成当前波次的所有敌人
            yield return StartCoroutine(SpawnWave(currentLevel.waves[waveIndex]));

            // 等待所有敌人被消灭
            yield return new WaitUntil(() => enemiesAlive <= 0);

            Debug.Log($"第 {waveIndex + 1} 波完成！");

            // 波次之间的额外等待时间（可选）
            if (waveIndex < currentLevel.waves.Length - 1)
            {
                yield return new WaitForSeconds(5f); // 5秒准备时间
            }
        }

        Debug.Log($"关卡 {currentLevel.levelName} 完成！");
        // 这里可以触发关卡胜利逻辑
    }

    /// <summary>
    /// 生成一波敌人
    /// </summary>
    IEnumerator SpawnWave(LevelConfig.WaveData wave)
    {
        // 波次开始前的准备时间
        yield return new WaitForSeconds(wave.delayBeforeWave);

        // 创建生成列表
        List<EnemyData> spawnList = new List<EnemyData>();

        // 将所有要生成的怪物加入列表
        foreach (var entry in wave.enemies)
        {
            for (int i = 0; i < entry.count; i++)
            {
                spawnList.Add(entry.enemyData);
            }
        }

        // 随机打乱顺序
        for (int i = 0; i < spawnList.Count; i++)
        {
            int randomIndex = Random.Range(i, spawnList.Count);
            EnemyData temp = spawnList[i];
            spawnList[i] = spawnList[randomIndex];
            spawnList[randomIndex] = temp;
        }

        // 按随机顺序生成
        foreach (var enemyData in spawnList)
        {
            SpawnEnemy(enemyData);
            yield return new WaitForSeconds(wave.enemies[0].spawnInterval);
        }
    }

    /// <summary>
    /// 生成单个敌人
    /// </summary>
    private void SpawnEnemy(EnemyData enemyData)
    {
        if (enemyData.enemyPrefab == null)
        {
            Debug.LogError($"EnemyData {enemyData.enemyName} 没有设置预制体！");
            return;
        }

        GameObject enemyObj = Instantiate(
            enemyData.enemyPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        Enemy enemy = enemyObj.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Init(enemyData);
            enemiesAlive++;  // 增加存活敌人计数
        }
        else
        {
            Debug.LogError("敌人预制体上缺少 Enemy 组件！");
        }
    }

    /// <summary>
    /// 敌人死亡时调用（由 Enemy 类调用）
    /// </summary>
    public void OnEnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0; // 防止负数
    }
}
