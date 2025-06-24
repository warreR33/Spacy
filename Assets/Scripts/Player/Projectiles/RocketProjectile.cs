using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float rotateSpeed = 200f;
    public int damage;
    private Transform target;
    private string targetTag;

    private float targetSearchInterval = 0.2f;
    private float searchTimer = 0f;

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    public void SetTargetTag(string tag)
    {
        targetTag = tag;
        target = FindClosestTarget();
    }

    void Update()
    {
        if (target == null)
            {
                searchTimer += Time.deltaTime;
                if (searchTimer >= targetSearchInterval)
                {
                searchTimer = 0f;
                target = FindClosestTarget();
            }

            transform.Translate(Vector2.up * speed * Time.deltaTime);
            return;
        }

        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        transform.Rotate(0, 0, -rotateAmount * rotateSpeed * Time.deltaTime);

        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }


    Transform FindClosestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(targetTag);
        Transform closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(currentPos, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy.transform;
            }
        }

        return closest;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (target != null && collision.gameObject == target.gameObject)
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
