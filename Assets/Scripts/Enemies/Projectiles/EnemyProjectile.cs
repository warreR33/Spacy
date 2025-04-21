using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
