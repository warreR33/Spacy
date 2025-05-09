using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatling : BaseWeapon
{
    public override void Shoot()
    {
        Transform point = GetNextShootPoint();
        if (point == null) return;

        GameObject plasma = Instantiate(projectilePrefab, point.position, point.rotation);
        PlasmaProjectile proj = plasma.GetComponent<PlasmaProjectile>();
        if (proj != null)
        {
            proj.SetDamage(damage);
        }
    }
}
