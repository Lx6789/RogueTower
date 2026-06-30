using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Base : MonoBehaviour
{
    private int maxBaseHealth;
    private int currenBaseHealth;
    private int currentLevelIndex = 0;
    private TextMeshPro baseHealthText;

    public int CurrentHealth => currenBaseHealth;

    private void Start()
    {
        maxBaseHealth = GameManager.Instance.LevelList.levels[currentLevelIndex].baseHealth;
        currenBaseHealth = maxBaseHealth;
        Transform child = transform.Find("BaseHealth");
        if (child != null)
        {
            baseHealthText = child.GetComponent<TextMeshPro>();
        }
        else
        {
            Debug.LogError("未找到子物体 BaseHealth");
        }
        UpdateBaseHealthText();
    }


    /// <summary>
    /// 对城堡造成伤害
    /// </summary>
    /// <param name="damage"></param>
    public void GetDamageOfBase(int damage)
    {
        if (currenBaseHealth > 0)
        {
            currenBaseHealth -= damage;
            UpdateBaseHealthText();

            if (currenBaseHealth <= 0)
            {
                GameManager.Instance.GameOver(currenBaseHealth);
            } 
        }
    }

    /// <summary>
    /// 修改塔的血量文本
    /// </summary>
    private void UpdateBaseHealthText()
    {
        baseHealthText.text = $"{currenBaseHealth}/{maxBaseHealth}";
    }
}
