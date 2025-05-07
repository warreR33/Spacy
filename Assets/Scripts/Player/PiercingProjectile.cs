using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage;
    public string objetiveName;

    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

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
        if (collision.CompareTag(objetiveName) && !alreadyHit.Contains(collision.gameObject))
        {
            alreadyHit.Add(collision.gameObject);
            collision.GetComponent<HealthSystem>()?.TakeDamage(damage);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
