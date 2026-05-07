using UnityEngine;

public class Base : MonoBehaviour
{
    [Header("血量")]
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// 炮台扣血
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Debug.Log("游戏结束");
        }
    }
}