using System.Collections.Generic;
using UnityEngine;

public enum FormationType
{
    Random,
    HorizontalLine,
    VerticalLine,
    Square,
    Circle
}

public static class SpawnPatterns
{
    public static List<Vector2Int> GetFormation(Vector2Int gridSize, bool[,] occupied, FormationType type, int count, int maxAttempts = 20)
    {
        switch (type)
        {
            case FormationType.Random:
                return GetRandomFreeCells(gridSize, occupied, count);
            default:
                return TryPattern(gridSize, occupied, type, count, maxAttempts);
        }
    }

    private static List<Vector2Int> GetRandomFreeCells(Vector2Int gridSize, bool[,] occupied, int count)
    {
        List<Vector2Int> free = new List<Vector2Int>();

        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++)
                if (!occupied[x, y])
                    free.Add(new Vector2Int(x, y));

        Shuffle(free);
        return free.GetRange(0, Mathf.Min(count, free.Count));
    }

    private static List<Vector2Int> TryPattern(Vector2Int gridSize, bool[,] occupied, FormationType type, int count, int maxAttempts)
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2Int origin = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
            List<Vector2Int> positions = GenerateOffsets(origin, type, count);

            if (AreCellsFree(positions, gridSize, occupied))
                return positions;
        }

        return new List<Vector2Int>();
    }

    private static List<Vector2Int> GenerateOffsets(Vector2Int origin, FormationType type, int count)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        switch (type)
        {
            case FormationType.HorizontalLine:
                for (int i = 0; i < count; i++)
                    positions.Add(new Vector2Int(origin.x + i, origin.y));
                break;

            case FormationType.VerticalLine:
                for (int i = 0; i < count; i++)
                    positions.Add(new Vector2Int(origin.x, origin.y + i));
                break;

            case FormationType.Square:
                int side = Mathf.CeilToInt(Mathf.Sqrt(count));
                for (int dx = 0; dx < side; dx++)
                    for (int dy = 0; dy < side && positions.Count < count; dy++)
                        positions.Add(new Vector2Int(origin.x + dx, origin.y + dy));
                break;

            case FormationType.Circle:
                int radius = Mathf.CeilToInt(Mathf.Sqrt(count / Mathf.PI));
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        if (positions.Count >= count) break;
                        if (dx * dx + dy * dy <= radius * radius)
                            positions.Add(new Vector2Int(origin.x + dx, origin.y + dy));
                    }
                }
                break;
        }

        return positions;
    }

    private static bool AreCellsFree(List<Vector2Int> positions, Vector2Int gridSize, bool[,] occupied)
    {
        foreach (var pos in positions)
        {
            if (pos.x < 0 || pos.x >= gridSize.x || pos.y < 0 || pos.y >= gridSize.y)
                return false;
            if (occupied[pos.x, pos.y])
                return false;
        }
        return true;
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}