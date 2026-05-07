using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("玩家资源")]
    [SerializeField] private int gold = 200;
    [SerializeField] private int score = 0;

    // 金币变化事件（UI 订阅）
    public System.Action<int> OnGoldChanged;
    public System.Action<int> OnScoreChanged;

    public int Gold => gold;
    public int Score => score;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 加金币
    /// </summary>
    /// <param name="amount"></param>
    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }

    /// <summary>
    /// 花金币，不够返回 false
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            OnGoldChanged?.Invoke(gold);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 加分
    /// </summary>
    /// <param name="amount"></param>
    public void AddScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);
    }
}
