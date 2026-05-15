using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("当前关卡配置")]
    private LevelList levelList;
    [Header("怪物出生点")]
    public Transform spawnPoint;

    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 确保GameManager已初始化
        if (GameManager.Instance == null || GameManager.Instance.LevelList == null)
        {
            Debug.LogError("GameManager 或 LevelList 未正确初始化");
            return;
        }

        levelList = GameManager.Instance.LevelList;
        int levelIndex = GameManager.Instance.CurrentLevelIndex;
        StartCoroutine(ProcessLevel(levelIndex));
    }

    IEnumerator ProcessLevel(int levelIndex)
    {
        if (levelList == null || levelIndex >= levelList.levels.Length)
        {
            Debug.LogError("无效的关卡索引！");
            yield break;
        }

        LevelConfig currentLevel = levelList.levels[levelIndex];
        Debug.Log($"开始关卡: {currentLevel.levelName}");

        for (int waveIndex = 0; waveIndex < currentLevel.waves.Length; waveIndex++)
        {
            if (GameManager.Instance.IsGameOver)
                yield break;

            Debug.Log($"准备第 {waveIndex + 1} 波");
            yield return StartCoroutine(SpawnWave(currentLevel.waves[waveIndex]));

            yield return new WaitUntil(() => enemiesAlive <= 0 || GameManager.Instance.IsGameOver);

            if (GameManager.Instance.IsGameOver)
                yield break;

            Debug.Log($"第 {waveIndex + 1} 波完成！");

            if (waveIndex < currentLevel.waves.Length - 1)
            {
                yield return new WaitForSeconds(5f);
            }
        }

        if (!GameManager.Instance.IsGameOver)
        {
            Base baseComp = GameManager.Instance.baseTransform?.GetComponent<Base>();
            int baseHealth = baseComp != null ? baseComp.CurrentHealth : 1;
            GameManager.Instance.GameOver(baseHealth);
        }
    }

    IEnumerator SpawnWave(LevelConfig.WaveData wave)
    {
        yield return new WaitForSeconds(wave.delayBeforeWave);

        if (GameManager.Instance.IsGameOver)
            yield break;

        List<EnemyData> spawnList = new List<EnemyData>();

        foreach (var entry in wave.enemies)
        {
            for (int i = 0; i < entry.count; i++)
            {
                spawnList.Add(entry.enemyData);
            }
        }

        for (int i = 0; i < spawnList.Count; i++)
        {
            int randomIndex = Random.Range(i, spawnList.Count);
            EnemyData temp = spawnList[i];
            spawnList[i] = spawnList[randomIndex];
            spawnList[randomIndex] = temp;
        }

        foreach (var enemyData in spawnList)
        {
            if (GameManager.Instance.IsGameOver)
                yield break;

            SpawnEnemy(enemyData);
            yield return new WaitForSeconds(wave.enemies[0].spawnInterval);
        }
    }

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
            enemiesAlive++;
        }
        else
        {
            Debug.LogError("敌人预制体上缺少 Enemy 组件！");
        }
    }

    public void OnEnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0;
    }
}