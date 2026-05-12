using UnityEngine;

public enum TowerType
{
    FiredBullet,
    AreaAttack,
    ContinuousAttack
}

[CreateAssetMenu(fileName = "TowerData", menuName = "塔数据配置")]
public class TowerData : ScriptableObject
{
    [Header("基础信息")]
    public string towerName;
    [TextArea] public string descript;
    public Sprite towerIcon;
    public TowerType type;
    public int maxLevel;
    public GameObject towerPrefab;

    [Header("战斗属性")]
    [Tooltip("伤害（发射型/范围型：每次伤害；持续型：每秒伤害）")]
    [Min(1)] public int damage = 10;

    [Tooltip("攻击范围")]
    public float range = 3f;

    [Tooltip("攻击间隔（发射型/范围型：两次攻击间隔；持续型：不使用）")]
    public float attackInterval = 1.5f;

    [Tooltip("建造金额")]
    [Min(0)] public int cost = 100;

    [Header("范围攻击专用")]
    [Tooltip("子弹扩张时间（秒）")]
    public float expandDuration = 1f;

    [Header("持续攻击专用")]
    [Tooltip("伤害间隔（秒）")]
    public float tickInterval = 0.3f;

    [Header("子弹相关")]
    [Tooltip("子弹飞行速度（发射型用）")]
    public float bulletSpeed = 10f;

    [Tooltip("子弹预制体")]
    public GameObject bulletPrefab;

    [Tooltip("击中特效")]
    public GameObject BulletEffect;
}