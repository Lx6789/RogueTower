using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Text goldText;
    [SerializeField] private Text scoreText;

    void Start()
    {
        // 初始显示
        UpdateGold(GameManager.Instance.Gold);
        UpdateScore(GameManager.Instance.Score);

        // 订阅变化
        GameManager.Instance.OnGoldChanged += UpdateGold;
        GameManager.Instance.OnScoreChanged += UpdateScore;
    }

    void UpdateGold(int value) => goldText.text = $"金币: {value}";
    void UpdateScore(int value) => scoreText.text = $"分数: {value}";

    void OnDestroy()
    {
        // 取消订阅，防止空引用
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGoldChanged -= UpdateGold;
            GameManager.Instance.OnScoreChanged -= UpdateScore;
        }
    }
}
