using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 2f;

    public Vector2 origin = Vector2.zero;

    private bool[,] occupiedCells;

    private void Awake()
    {
        occupiedCells = new bool[gridWidth, gridHeight];
    }

    public Vector2 GetCellCenter(int x, int y)
    {
        Vector2 worldOrigin = (Vector2)transform.position + origin;
        return worldOrigin + new Vector2(x * cellSize + cellSize / 2f, y * cellSize + cellSize / 2f);
    }

    public Vector2 GetGroupCenter(Vector2Int originCell, EnemySize size)
    {
        int width = 1, height = 1;

        switch (size)
        {
            case EnemySize.Size2x1: width = 2; break;
            case EnemySize.Size1x2: height = 2; break;
            case EnemySize.Size2x2: width = 2; height = 2; break;
        }

        Vector2 bottomLeft = GetCellCenter(originCell.x, originCell.y) - new Vector2(cellSize / 2f, cellSize / 2f);
        Vector2 topRight = bottomLeft + new Vector2(width * cellSize, height * cellSize);
        return (bottomLeft + topRight) / 2f;
    }

    public bool IsCellOccupied(int x, int y)
    {
        return occupiedCells[x, y];
    }

    public bool[,] GetOccupiedArray()
    {
        bool[,] result = new bool[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                result[x, y] = occupiedCells[x, y];
            }
        }
        return result;
    }

    public void SetCellOccupied(int x, int y, bool occupied)
    {
        occupiedCells[x, y] = occupied;
    }

    public void SetCellsOccupied(Vector2Int origin, EnemySize size, bool occupied)
    {
        foreach (var cell in GetOccupiedCells(origin, size))
        {
            occupiedCells[cell.x, cell.y] = occupied;
        }
    }

    public bool AreCellsFree(Vector2Int origin, EnemySize size)
    {
        foreach (var cell in GetOccupiedCells(origin, size))
        {
            if (cell.x < 0 || cell.x >= gridWidth || cell.y < 0 || cell.y >= gridHeight)
                return false;

            if (occupiedCells[cell.x, cell.y])
                return false;
        }
        return true;
    }

    public List<Vector2Int> GetOccupiedCells(Vector2Int origin, EnemySize size)
    {
        int width = 1, height = 1;

        switch (size)
        {
            case EnemySize.Size2x1: width = 2; break;
            case EnemySize.Size1x2: height = 2; break;
            case EnemySize.Size2x2: width = 2; height = 2; break;
        }

        List<Vector2Int> cells = new List<Vector2Int>();
        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                cells.Add(new Vector2Int(origin.x + dx, origin.y + dy));
            }
        }
        return cells;
    }

    private void OnDrawGizmos()
    {
        if (occupiedCells == null)
        {
            occupiedCells = new bool[gridWidth, gridHeight];
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 center = origin + new Vector2(x * cellSize + cellSize / 2f, y * cellSize + cellSize / 2f);
                Gizmos.color = occupiedCells[x, y] ? Color.red : Color.green;
                Gizmos.DrawWireCube(center, Vector3.one * cellSize * 0.95f);
            }
        }
    }

    public Vector2Int? FindFreeArea(int width, int height)
    {
        for (int x = 0; x <= gridWidth - width; x++)
        {
            for (int y = 0; y <= gridHeight - height; y++)
            {
                bool areaFree = true;

                for (int dx = 0; dx < width && areaFree; dx++)
                {
                    for (int dy = 0; dy < height; dy++)
                    {
                        if (occupiedCells[x + dx, y + dy])
                        {
                            areaFree = false;
                            break;
                        }
                    }
                }

                if (areaFree)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return null; 
    }

    public List<Vector2Int> MarkAreaOccupied(int startX, int startY, int width, int height, bool occupied)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                int x = startX + dx;
                int y = startY + dy;

                if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
                {
                    occupiedCells[x, y] = occupied;
                    cells.Add(new Vector2Int(x, y));
                }
            }
        }

        return cells;
    }
}
