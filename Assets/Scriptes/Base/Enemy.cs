using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("基础信息")]
    [Tooltip("敌人名称，仅用于识别")]
    public string enemyName;
    [Tooltip("敌人描述")]
    [TextArea] public string descript;

    [Header("战斗属性")]
    [Tooltip("最大生命值")]
    [Min(1)]
    public int maxHealth;
    [Tooltip("移动速度（单位/秒）")]
    [Range(1f, 10f)]
    public float moveSpeed = 3.5f;
    [Tooltip("对基地的伤害")]
    public int damage;

    [Header("表现")]
    [Tooltip("敌人图片")]
    public Sprite enemyIcon;
    [Tooltip("死亡时生成的粒子特效")]
    public GameObject deathEffect;

    [Header("奖励")]
    [Tooltip("击杀后获得的经验值")]
    [Min(0)]
    public int rewardExp = 20;
    [Tooltip("击杀后获得的金币")]
    [Min(0)]
    public int rewardGold = 10;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// 怪物移动
    /// </summary>
    protected virtual void Move()
    {
        
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage"></param>
    private void getDamege(int damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    /// <summary>
    /// 敌人死亡
    /// </summary>
    private void Die()
    {
        Destroy(gameObject);
    }
}
