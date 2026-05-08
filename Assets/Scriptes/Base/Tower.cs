using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    [Header("基础信息")]
    [Tooltip("塔名称，仅用于识别")]
    public string enemyName;
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
