using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zapper : BaseWeapon
{
    public override void Shoot()
    {
        Transform point = GetNextShootPoint();
        if (point == null) return;

        GameObject projectile = Instantiate(projectilePrefab, point.position, point.rotation);
        PiercingProjectile proj = projectile.GetComponent<PiercingProjectile>();
        if (proj != null)
        {
            proj.damage = damage;
        }
    }
}
