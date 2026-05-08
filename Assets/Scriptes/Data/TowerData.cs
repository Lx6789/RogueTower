using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    FiredBullet,        // 发射子弹
    AreaAttack,         // 范围攻击
    ContinuousAttack    // 连线持续攻击
}

[CreateAssetMenu(fileName = "New TowerData", menuName = "塔数据配置")]
public class TowerData : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("塔名称，仅用于识别")]
    public string towerName;
    [Tooltip("塔描述")]
    [TextArea] public string descript;
    [Tooltip("塔图片")]
    public Sprite towerIcon;
    [Tooltip("塔类型")]
    public TowerType type;
    [Tooltip("等级上限")]
    public int maxLevel;

    [Header("战斗属性")]
    [Tooltip("塔伤害")]
    [Min(10)]
    public int damage;
    [Tooltip("塔伤害范围")]
    public float range;
    [Tooltip("塔攻击间隔")]
    public float attackInterval;
    [Tooltip("塔建造金额")]
    [Min(0)]
    public int cost;

    [Header("子弹相关")]
    [Tooltip("子弹图片")]
    public Sprite bulletIcon;
    [Tooltip("子弹攻击到敌人的粒子特效")]
    public GameObject BulletEffect;
}
