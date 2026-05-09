using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    private int maxBaseHealth;
    private int currenBaseHealth;
    private int currentLevelIndex = 0;

    private void Start()
    {
        maxBaseHealth = GameManager.Instance.LevelList.levels[currentLevelIndex].baseHealth;
        currenBaseHealth = maxBaseHealth;
    }

    public void getDamageOfBase(int damage)
    {
        if (currenBaseHealth > 0)
        {
            currenBaseHealth -= damage;

            if (currenBaseHealth <= 0)
            {
                Debug.Log("ÓÎÏ·½áÊø");
            } 
        }
    }
}
