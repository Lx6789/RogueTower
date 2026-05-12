using UnityEngine;

public abstract class FiredBullet : Bullet
{
    protected Transform target;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    protected virtual void Update()
    {
        MoveTowardsTarget();
    }

    protected virtual void MoveTowardsTarget()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;

        if (obstacleLayer != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, obstacleLayer);
            if (hit.collider != null)
            {
                Destroy(gameObject);
                return;
            }
        }

        if (rb != null)
        {
            Vector2 newPos = rb.position + direction * speed * Time.deltaTime;
            rb.MovePosition(newPos);
        }
        else
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsEnemy(collision)) return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            DealDamageToEnemy(enemy);
            ApplyEffect(enemy);
        }

        PlayHitEffect();
        Destroy(gameObject);
    }

    protected virtual void ApplyEffect(Enemy enemy) { }
}