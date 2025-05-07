using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public HealthSystem healthSystem;

    public EnemySize size = EnemySize.Size1x1;

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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip damageClip;
    public AudioClip deathClip;

    [Header("Events")]
    public System.Action OnDeath;

    [Header("Grid")]
    private GridManager gridManager;
    private List<Vector2Int> occupiedCells = new List<Vector2Int>();

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
        if (gridManager != null && occupiedCells != null)
        {
            foreach (var cell in occupiedCells)
            {
                gridManager.SetCellOccupied(cell.x, cell.y, false);
                Debug.Log($"Celda {cell} liberada por {gameObject.name}");
            }
        }
    }

    public void AssignCells(GridManager grid, Vector2Int originCell)
    {
        gridManager = grid;
        occupiedCells.Clear();

        int width = (int)size.GetSize().x;
        int height = (int)size.GetSize().y;

        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                Vector2Int cell = new Vector2Int(originCell.x + dx, originCell.y + dy);
                occupiedCells.Add(cell);
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
    
public enum EnemySize
{
    Size1x1,
    Size2x1,
    Size1x2,
    Size2x2
}

public static class EnemySizeExtensions
{
    public static Vector2 GetSize(this EnemySize size)
    {
        switch (size)
        {
            case EnemySize.Size2x1: return new Vector2(2, 1);
            case EnemySize.Size1x2: return new Vector2(1, 2);
            case EnemySize.Size2x2: return new Vector2(2, 2);
            default: return new Vector2(1, 1);
        }
    }
}
