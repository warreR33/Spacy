using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnemySet
{
    public int level;
    public List<GameObject> enemies;
}
public class EnemySpawner : MonoBehaviour
{
    [Header("Grid Reference")]
    public GridManager gridManager;

    [Header("Enemy Spawning")]
    public List<GameObject> enemyPrefabs;
    public float spawnInterval = 5f;
    public int minEnemiesPerWave = 2;
    public int maxEnemiesPerWave = 5;

    [Header("Formation")]
    public FormationType formationType = FormationType.Random;

    private Coroutine spawnCoroutine; 
    public List<BaseEnemy> activeEnemies = new List<BaseEnemy>();

    [Header("Enemy Sets por Nivel")]
    public List<EnemySet> enemySets;

    private Dictionary<int, List<GameObject>> enemyPrefabsByLevel = new Dictionary<int, List<GameObject>>();

    public delegate void WaveClearedHandler();
    public event WaveClearedHandler OnWaveCleared;

    private void Start()
    {
        foreach (var set in enemySets)
        {
            if (!enemyPrefabsByLevel.ContainsKey(set.level))
            {
                enemyPrefabsByLevel[set.level] = set.enemies;
            }
        }
    }

    public IEnumerator SpawnWaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            SpawnEnemyGroup();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemyGroup()
    {
        if (GameProgressManager.Instance == null)
        return;

        if (GameProgressManager.Instance.currentState != GameState.OnLevel &&
            GameProgressManager.Instance.currentState != GameState.Tutorial)
            return;

        int count = Random.Range(minEnemiesPerWave, maxEnemiesPerWave + 1);
        Vector2Int gridSize = new Vector2Int(gridManager.gridWidth, gridManager.gridHeight);
        bool[,] occupied = gridManager.GetOccupiedArray();

        List<Vector2Int> spawnCells = SpawnPatterns.GetFormation(gridSize, occupied, formationType, count);

        foreach (var cell in spawnCells)
        {
            int level = GameProgressManager.Instance?.currentLevel ?? 0;
            if (!enemyPrefabsByLevel.ContainsKey(level)) return;

            List<GameObject> currentList = enemyPrefabsByLevel[level];
            if (currentList == null || currentList.Count == 0) return;

            GameObject prefab = currentList[Random.Range(0, currentList.Count)];
            BaseEnemy tempEnemy = prefab.GetComponent<BaseEnemy>();
            if (tempEnemy == null) continue;

            Vector2Int size = Vector2Int.RoundToInt(tempEnemy.size.GetSize());

            if (!gridManager.AreCellsFree(cell, tempEnemy.size))
                continue;

            gridManager.MarkAreaOccupied(cell.x, cell.y, size.x, size.y, true);
            Vector2 targetPosition = gridManager.GetGroupCenter(cell, tempEnemy.size);
            Vector2 spawnPosition = GetOffScreenSpawnPosition();

            GameObject enemyGO = Instantiate(prefab, spawnPosition, Quaternion.identity);
            BaseEnemy enemyScript = enemyGO.GetComponent<BaseEnemy>();
            if (enemyScript != null)
            {
                enemyScript.AssignCells(gridManager, cell);
                enemyScript.Initialize(targetPosition);

                activeEnemies.Add(enemyScript);
                enemyScript.OnEnemyDied += HandleEnemyDeath;
            }
        }

        
    }

    private void HandleEnemyDeath(BaseEnemy enemy)
    {
        activeEnemies.Remove(enemy);

        if (activeEnemies.Count == 0)
        {
            OnWaveCleared?.Invoke();
        }
    }

    Vector2 GetOffScreenSpawnPosition()
    {
        float offset = 5f;
        Vector3 camPos = Camera.main.transform.position;
        float camHeight = 2f * Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        int edge = Random.Range(0, 3);
        Vector2 pos = Vector2.zero;

        switch (edge)
        {
            case 0: // arriba
                pos = new Vector2(Random.Range(camPos.x - camWidth / 2, camPos.x + camWidth / 2), camPos.y + camHeight / 2 + offset);
                break;
            case 2: // izquierda
                pos = new Vector2(camPos.x - camWidth / 2 - offset, Random.Range(camPos.y - camHeight / 2, camPos.y + camHeight / 2));
                break;
            case 3: // derecha
                pos = new Vector2(camPos.x + camWidth / 2 + offset, Random.Range(camPos.y - camHeight / 2, camPos.y + camHeight / 2));
                break;
        }

        return pos;
    }


    public void SpawnSingleWave()
    {
        SpawnEnemyGroup(); 
    }
}
