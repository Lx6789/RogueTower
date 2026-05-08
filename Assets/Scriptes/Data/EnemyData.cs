using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyType
{
    Normal,         //普通怪物
    Fast,           //跑的快的怪物
    Tank,           //很肉的怪物
    Flying,         //会飞的怪物
    Boss            //老大
}

[CreateAssetMenu(fileName = "New EnemyData", menuName = "敌人数据配置")]
public class EnemyData : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("敌人名称，仅用于识别")]
    public string enemyName;
    [Tooltip("敌人描述")]
    [TextArea] public string descript;
    [Tooltip("敌人类型")]
    public EnemyType enemyType;

    [Header("战斗属性")]
    [Tooltip("最大生命值")]
    [Min(1)]
    public int maxHealth = 100;
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
}
