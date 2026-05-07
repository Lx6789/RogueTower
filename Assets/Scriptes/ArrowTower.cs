using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTower : Tower
{

    protected override void Shoot()
    {
        if (bulletPrefab == null || currentTarget == null) return;

        GameObject bullet = Instantiate(bulletPrefab, gameObject.transform.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetTarget(currentTarget, damage);
        }
    }
}
