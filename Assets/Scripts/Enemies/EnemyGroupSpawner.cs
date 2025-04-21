using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EntryDirection
{
    Top, Right, Left, BottomLeft, TopRight, TopLeft, BottomRight
}

public enum FormationType
{
    LineHorizontal,
    LineVertical,
    Circle,
    TwoRows
}
public class EnemyGroupSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int enemyCount = 5;
    public float spacing = 1.5f;
    public FormationType formationType;
    public EntryDirection entryDirection; 

    public EnemyZoneManager.EnemyZone zone;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        SpawnGroup();
    }

    void SpawnGroup()
    {
        List<Vector3> targetPositions = GenerateFormationPositions(formationType, enemyCount);

        foreach (Vector3 targetPos in targetPositions)
        {
            Vector3 spawnPos = GetSpawnPosition(entryDirection, targetPos);
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            BaseEnemy baseEnemy = enemy.GetComponent<BaseEnemy>();

            if (baseEnemy != null)
            {
                baseEnemy.Initialize(targetPos, shoot: Random.value > 0.5f);

                if (zone != null)
                {
                    baseEnemy.myZone = zone;
                    zone.activeEnemies.Add(enemy);
                    zone.isOccupied = true;
                }
            }
        }
    }

    List<Vector3> GenerateFormationPositions(FormationType type, int count)
    {
        List<Vector3> positions = new List<Vector3>();  

        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        Vector3 center = zone != null ? zone.Position : 
            new Vector3(mainCam.transform.position.x, mainCam.transform.position.y - camHeight / 4, 0f);

        float zoneWidth = zone != null ? zone.Width : camWidth;
        float zoneHeight = zone != null ? zone.Height : camHeight;

        switch (type)
        {
            case FormationType.LineHorizontal:
                float maxHorizontal = Mathf.Min(zoneWidth, spacing * (count - 1));
                for (int i = 0; i < count; i++)
                {
                    float x = center.x - (maxHorizontal / 2f) + i * spacing;
                    float y = center.y;
                    positions.Add(new Vector3(x, y, 0f));
                }
                break;

            // case FormationType.LineVertical:
            //     float maxVertical = Mathf.Min(zoneHeight, spacing * (count - 1));
            //     for (int i = 0; i < count; i++)
            //     {
            //         float y = center.y - (maxVertical / 2f) + i * spacing;
            //         float x = center.x;
            //         positions.Add(new Vector3(x, y, 0f));
            //     }
            //     break;

            case FormationType.Circle:
                float radius = Mathf.Min(zoneWidth, zoneHeight) / 2.5f;
                for (int i = 0; i < count; i++)
                {
                    float angle = i * Mathf.PI * 2 / count;
                    float x = center.x + Mathf.Cos(angle) * radius;
                    float y = center.y + Mathf.Sin(angle) * radius;
                    positions.Add(new Vector3(x, y, 0f));
                }
                break;

            case FormationType.TwoRows:
                int half = count / 2;
                float rowYSpacing = zoneHeight * 0.10f;
                for (int i = 0; i < count; i++)
                {
                    float rowOffset = (i < half) ? rowYSpacing : -rowYSpacing;
                    float maxRowWidth = Mathf.Min(zoneWidth, spacing * (half - 1));
                    float x = center.x - (maxRowWidth / 2f) + (i % half) * spacing;
                    float y = center.y + rowOffset;
                    positions.Add(new Vector3(x, y, 0f));
                }
                break;
        }

        return positions;
    }

    Vector3 GetSpawnPosition(EntryDirection direction, Vector3 target)
    {
        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        Vector3 camPos = mainCam.transform.position;

        Vector3 spawnPos = target;

        switch (direction)
        {
            case EntryDirection.Top:
                spawnPos = new Vector3(target.x, camPos.y + camHeight + 2f, 0f);
                break;
            case EntryDirection.Right:
                spawnPos = new Vector3(camPos.x + camWidth + 2f, target.y, 0f);
                break;
            case EntryDirection.Left:
                spawnPos = new Vector3(camPos.x - camWidth - 2f, target.y, 0f);
                break;
            case EntryDirection.BottomLeft:
                spawnPos = new Vector3(camPos.x - camWidth - 2f, camPos.y - camHeight - 2f, 0f);
                break;
            case EntryDirection.TopRight:
                spawnPos = new Vector3(camPos.x + camWidth + 2f, camPos.y + camHeight + 2f, 0f);
                break;
            case EntryDirection.TopLeft:
                spawnPos = new Vector3(camPos.x - camWidth - 2f, camPos.y + camHeight + 2f, 0f);
                break;
            case EntryDirection.BottomRight:
                spawnPos = new Vector3(camPos.x + camWidth + 2f, camPos.y - camHeight - 2f, 0f);
                break;
        }

        return spawnPos;
    }

    void OnDestroy()
    {
        if (zone != null)
            zone.isOccupied = false;
    }
}
