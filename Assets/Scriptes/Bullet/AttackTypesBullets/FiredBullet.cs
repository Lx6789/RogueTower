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
        if (GameManager.IsPaused) return;
        MoveTowardsTarget();
    }

    protected virtual void MoveTowardsTarget()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            ReleaseBullet();
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;

        if (obstacleLayer != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, obstacleLayer);
            if (hit.collider != null)
            {
                ReleaseBullet();
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

        // 播放一次性命中特效
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, hitEffect.transform.rotation);
            var sfx = effect.GetComponent<SFXPlayerViaManager>();
            if (sfx != null)
            {
                // 一次性命中音效，不循环
                sfx.InitAndPlay(clip, false);
            }
            Destroy(effect, 2f);
        }

        ReleaseBullet();
    }

    protected virtual void ApplyEffect(Enemy enemy) { }
}