using UnityEngine;

public class SunBullet : AreaAttackBullet
{
    private int burnDamage;
    private float burnDuration;

    public void SetBurn(int damage, float duration)
    {
        burnDamage = damage;
        burnDuration = duration;
    }

    protected override void ApplyEffect(Enemy enemy)
    {
        if (burnDamage > 0)
            StartCoroutine(ApplyBurn(enemy));
    }

    private System.Collections.IEnumerator ApplyBurn(Enemy enemy)
    {
        float elapsed = 0;
        while (elapsed < burnDuration)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) yield break;
            enemy.TakeDamage(burnDamage);
            elapsed += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
    }
}