using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public HealthSystem healthSystem;

    [Header("Stats")]
    public int pointsOnDeath = 10;
    public float moveSpeed = 2f;

    [Header("Shooting")]
    public bool canShoot = false;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    private float shootTimer;

    [Header("Movement")]
    private Vector3 targetPosition;
    private bool isMoving = true;

    [Header("Zone Management")]
    public EnemyZoneManager.EnemyZone myZone;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip damageClip;
    public AudioClip deathClip;

    [Header("Events")]
    public System.Action OnDeath;

    public SpriteRenderer render;
    public Collider2D enemyCollider;

    public void Initialize(Vector3 target, bool shoot = false)
    {
        targetPosition = target;
        canShoot = shoot;
        isMoving = true;
    }

    void Start()
    {
        healthSystem.OnHealthChanged += (current, max) =>
        {
            if (!healthSystem.IsDead && current < max) 
            {
                audioSource.PlayOneShot(damageClip);        
            }
        };

        healthSystem.OnDeath += HandleDeath;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
                canShoot = true;
            }
        }

        if (canShoot && !isMoving)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                Shoot();
                shootTimer = 0f;
            }
        }
    }

    void OnDestroy()
    {
        if (myZone != null)
        {
            myZone.activeEnemies.Remove(gameObject);
            if (myZone.activeEnemies.Count == 0)
            {
                myZone.isOccupied = false;
                Debug.Log($"Zona {myZone.name} liberada.");
            }
        }
    }

    private void HandleDeath()
    {
        Points.Instance.AddPoints(pointsOnDeath);
        OnDeath?.Invoke();
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        if (enemyCollider != null) enemyCollider.enabled = false;
        if (deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
            Color originalColor = render.color;
            render.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            render.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }
        Destroy(gameObject);
    }

    private void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        }
    }
    
}
