using UnityEngine;

public class ArrowTower : FiredBulletTower
{
    [Header("箭塔特性")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool smoothRotation = true;  // 可选：是否平滑旋转

    private Quaternion targetRotation;
    private bool hasTarget = false;

    protected override void OnStart()
    {
        targetRotation = transform.rotation;
    }

    protected override void OnUpdate()
    {
        UpdateTargetRotation();
        ApplyRotation();
    }

    private void UpdateTargetRotation()
    {
        if (currentTarget != null)
        {
            Vector2 direction = currentTarget.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(0, 0, angle);
            hasTarget = true;
        }
        else
        {
            hasTarget = false;
        }
    }

    private void ApplyRotation()
    {
        if (!hasTarget) return;

        if (smoothRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }
}