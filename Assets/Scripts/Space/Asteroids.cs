using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroids : MonoBehaviour
{
    public int damage = 1;
    void Update()
    {
        if (!IsInExtendedCameraBounds(Camera.main, 2f))
        {
            Destroy(gameObject);
        }
    }

    private bool IsInExtendedCameraBounds(Camera cam, float extraMargin)
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        return viewportPos.x >= -extraMargin && viewportPos.x <= 1 + extraMargin &&
               viewportPos.y >= -extraMargin && viewportPos.y <= 1 + extraMargin;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthSystem>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

}
