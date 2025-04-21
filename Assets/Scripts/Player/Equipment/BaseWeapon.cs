using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int damage;
    public float cooldown = 0.5f;
    public Transform[] shootPoints;


    private float timer = 0f;

    public void UpdateWeapon()
    {
        timer += Time.deltaTime;
        if (timer >= cooldown)
        {
            Shoot();
            timer = 0f;
        }
    }

    public abstract void Shoot();
}
