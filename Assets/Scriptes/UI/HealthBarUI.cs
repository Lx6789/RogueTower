using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 0.25f, 0); // Í·¶¥Æ«ÒÆ

    public void Init(Transform targetTransform, int maxHealth)
    {
        target = targetTransform;
        fill.fillAmount = 1f;
        fill.color = Color.red;    // ³õÊ¼ÂÌÉ«
        UpdatePosition();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        transform.position = target.position + worldOffset;
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        float percent = (float)currentHealth / maxHealth;
        fill.fillAmount = percent;
    }
}