using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    public GameObject enemyGroupSpawnerPrefab;

    public EnemyZoneManager zoneManager;
    
    public float spawnInterval = 5f;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnRandomGroup();
            timer = 0f;
        }
    }

    void SpawnRandomGroup()
    {
        var zone = zoneManager.GetAvailableZone();

        if (zone == null)
        {
            Debug.Log("No hay zonas disponibles para spawnear.");
            return;
        }

        zone.isOccupied = true;

        GameObject spawnerObj = Instantiate(enemyGroupSpawnerPrefab);
        EnemyGroupSpawner spawner = spawnerObj.GetComponent<EnemyGroupSpawner>();

        spawner.enemyCount = Random.Range(3, 10);

        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        float maxSafeSpacing = (camWidth * 1.5f) / spawner.enemyCount;

        spawner.spacing = Mathf.Clamp(Random.Range(0.6f, 1.2f), 0.4f, maxSafeSpacing);
        
        spawner.formationType = (FormationType)Random.Range(0, System.Enum.GetValues(typeof(FormationType)).Length);
        spawner.entryDirection = (EntryDirection)Random.Range(0, System.Enum.GetValues(typeof(EntryDirection)).Length);

        spawner.zone = zone;
    }
}
