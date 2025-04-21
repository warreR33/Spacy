using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage;
    public string objetiveName;

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }
    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag($"{objetiveName}"))
        {
            collision.GetComponent<HealthSystem>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
