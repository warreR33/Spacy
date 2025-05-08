using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemySize
{
    public int width;
    public int height;

    public EnemySize(int w, int h)
    {
        width = w;
        height = h;
    }

    public Vector2 GetSize() => new Vector2(width, height);
}

public class BaseEnemy : MonoBehaviour
{
    public HealthSystem healthSystem;

    public EnemySize size = new EnemySize(1, 1);

    [Header("Stats")]
    public int pointsOnDeath = 10;
    public float moveSpeed = 2f;

    [Header("Shooting")]
    public List<Transform> firePoints = new List<Transform>();
    public bool canShoot = false;
    public GameObject projectilePrefab;
    public float shootInterval = 2f;
    private float shootTimer;
    public bool fireAllAtOnce  = false;
    private int currentFirePointIndex = 0;

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

    [Header("Animation")]
    public Animator animator;
    public Transform visualTransform;

    public SpriteRenderer render;
    public Collider2D enemyCollider;

    private Coroutine damageFlashCoroutine;

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
                if (damageFlashCoroutine != null)
                    StopCoroutine(damageFlashCoroutine);
                damageFlashCoroutine = StartCoroutine(FlashRed());

                if (damageClip != null)
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
                animator.SetTrigger("Shoot");
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

        if (enemyCollider != null) enemyCollider.enabled = false;

        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);

        if (animator != null)
            animator.SetTrigger("Die");
        else
            Destroy(gameObject); 
    }

    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }

    public void Shoot()
    {
        if (projectilePrefab == null || firePoints.Count == 0) return;

        if (fireAllAtOnce)
        {
            foreach (var firePoint in firePoints)
            {
                Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            }
        }
        else
        {
            Transform firePoint = firePoints[currentFirePointIndex];
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            currentFirePointIndex = (currentFirePointIndex + 1) % firePoints.Count;
        }
    }

    private IEnumerator FlashRed()
    {
        Color originalColor = render.color;
        Vector3 originalLocalPos = visualTransform.localPosition;

        float duration = 0.1f;
        float elapsed = 0f;
        float magnitude = 0.05f;

        render.color = Color.red;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-magnitude, magnitude);
            float offsetY = Random.Range(-magnitude, magnitude);
            visualTransform.localPosition = originalLocalPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        visualTransform.localPosition = originalLocalPos;
        render.color = originalColor;
    }
}


