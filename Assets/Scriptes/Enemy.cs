using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("血量")]
    [SerializeField]
    [Tooltip("最大血量")]
    private int maxHealth = 10;
    [SerializeField]
    [Tooltip("当前血量")]
    private int currentHealth;

    [Header("伤害相关")]
    [SerializeField]
    private int baseDamage = 1;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Base"))
        {
            Base baseScript = other.GetComponent<Base>();
            if (baseScript != null)
            {
                baseScript.TakeDamage(baseDamage);
            }
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 敌人扣血
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }
    }

    /// <summary>
    /// 敌人死亡
    /// </summary>
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
