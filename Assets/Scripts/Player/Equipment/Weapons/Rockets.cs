using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rockets : BaseWeapon
{
    public string targetTag = "Enemy";

    public override void Shoot()
    {
        if (shootPoints.Length == 0) return;
        
        Transform shootPoint = shootPoints[currentShootIndex];
        currentShootIndex = (currentShootIndex + 1) % shootPoints.Length;

        GameObject rocket = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

        RocketProjectile homing = rocket.GetComponent<RocketProjectile>();
        if (homing != null)
        {
            homing.SetDamage(damage);
            homing.SetTargetTag(targetTag);
        }
    }
}
