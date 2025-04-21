using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AsteroidMovementType
{
    Straight,
    Parabola
}

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject[] asteroidPrefabs;
    public float spawnDistance = 2f;
    public Camera mainCam;

    public float spawnInterval = 2f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnAsteroid();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnAsteroid()
    {
        GameObject prefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
        int side = Random.Range(0, 5);
        Vector2 spawnPos = Vector2.zero;
        Vector2 direction = Vector2.zero;

        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        switch (side)
        {
            case 0: // Arriba
                spawnPos = new Vector2(Random.Range(-camWidth, camWidth), mainCam.transform.position.y + camHeight + spawnDistance);
                direction = Vector2.down;
                break;
            case 1: // Derecha
                spawnPos = new Vector2(mainCam.transform.position.x + camWidth + spawnDistance, Random.Range(-camHeight, camHeight));
                direction = Vector2.left;
                break;
            case 2: // Izquierda
                spawnPos = new Vector2(mainCam.transform.position.x - camWidth - spawnDistance, Random.Range(-camHeight, camHeight));
                direction = Vector2.right;
                break;
            case 3: // Esquina superior derecha (↙)
                spawnPos = new Vector2(mainCam.transform.position.x + camWidth + spawnDistance, mainCam.transform.position.y + camHeight + spawnDistance);
                direction = new Vector2(-1, -1).normalized;
                break;
            case 4: // Esquina superior izquierda (↘)
                spawnPos = new Vector2(mainCam.transform.position.x - camWidth - spawnDistance, mainCam.transform.position.y + camHeight + spawnDistance);
                direction = new Vector2(1, -1).normalized;
                break;
        }

        GameObject asteroid = Instantiate(prefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float moveSpeed = Random.Range(1.5f, 3f);
            Vector2 force = direction * moveSpeed;

            AsteroidMovementType type = (AsteroidMovementType)Random.Range(0, 2);
            if (type == AsteroidMovementType.Parabola)
            {
                Vector2 curveForce = new Vector2(-direction.y, direction.x) * Random.Range(0.5f, 1.5f); // Fuerza perpendicular
                force += curveForce;
            }

            rb.velocity = force;
            rb.angularVelocity = Random.Range(-90f, 90f);
        }
    }
}
