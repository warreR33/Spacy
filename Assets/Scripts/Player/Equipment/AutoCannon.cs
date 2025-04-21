using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCannon : BaseWeapon
{
    public override void Shoot()
    {
        foreach (Transform point in shootPoints)
        {
            GameObject bullet = Instantiate(projectilePrefab, point.position, point.rotation);
            
            Projectile proj = bullet.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.SetDamage(damage);
            }
        }
    }
}
