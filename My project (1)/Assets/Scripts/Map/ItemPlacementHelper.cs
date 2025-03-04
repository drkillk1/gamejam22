using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPlacementHelper
{
    private HashSet<Vector2Int> occupiedTiles = new HashSet<Vector2Int>();
    Dictionary<PlacementType, HashSet<Vector2Int>> tileByType = new Dictionary<PlacementType, HashSet<Vector2Int>>();
    HashSet<Vector2Int> roomFloorNoCorridor; // Ensure rooms are separated from corridors

    public ItemPlacementHelper(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> corridors)
    {
        Graph graph = new Graph(roomFloor);
        roomFloorNoCorridor = new HashSet<Vector2Int>(roomFloor.Except(corridors)); // Exclude corridors

        foreach (var position in roomFloorNoCorridor)
        {
            int neighboursCount8Dir = graph.GetNeighbours8Directions(position).Count;
            PlacementType type = neighboursCount8Dir < 8 ? PlacementType.NearWall : PlacementType.OpenSpace;

            if (!tileByType.ContainsKey(type))
                tileByType[type] = new HashSet<Vector2Int>();

            tileByType[type].Add(position);
        }

        Debug.Log($"ItemPlacementHelper initialized. Room tiles: {roomFloorNoCorridor.Count}, OpenSpace: {tileByType.GetValueOrDefault(PlacementType.OpenSpace)?.Count ?? 0}");
    }

    public Vector2? GetItemPlacementPosition(PlacementType placementType, int iterationsMax, Vector2Int size, bool addOffset)
    {
        if (!tileByType.ContainsKey(placementType) || tileByType[placementType].Count == 0)
        {
            Debug.LogWarning($"No valid positions found for {placementType}. Trying fallback.");
            return null;
        }

        int iteration = 0;
        while (iteration < iterationsMax)
        {
            iteration++;
            int count = tileByType[placementType].Count;
            if (count == 0) return null; // Prevent out-of-range errors

            int index = UnityEngine.Random.Range(0, count);
            Vector2Int position = tileByType[placementType].ElementAt(index);

            // Ensure the tile is not already occupied
            if (occupiedTiles.Contains(position))
            {
                continue; // Skip this position and try again
            }

            // Mark the tile as occupied
            occupiedTiles.Add(position);

            return position;
        }

        return null;
    }

    private (bool result, IEnumerable<Vector2Int> placementPositions) PlaceBigItem(Vector2Int originPosition, Vector2Int size, bool addOffset)
    {
        List<Vector2Int> positions = new List<Vector2Int>() { originPosition };
        int maxX = addOffset ? size.x + 1 : size.x;
        int maxY = addOffset ? size.y + 1 : size.y;
        int minX = addOffset ? -1 : 0;
        int minY = addOffset ? -1 : 0;

        int validTiles = 0;
        for (int row = minX; row <= maxX; row++)
        {
            for (int col = minY; col <= maxY; col++)
            {
                if (col == 0 && row == 0) continue;
                
                Vector2Int newPosToCheck = new Vector2Int(originPosition.x + row, originPosition.y + col);

                // Instead of failing entirely, count valid tiles
                if (roomFloorNoCorridor.Contains(newPosToCheck))
                {
                    validTiles++;
                    positions.Add(newPosToCheck);
                }
            }
        }

        // Allow placement if at least 75% of the tiles are valid
        if (validTiles >= (size.x * size.y * 0.75f))
            return (true, positions);

        return (false, positions);
    }

        public enum PlacementType
    {
        OpenSpace,
        NearWall
    }
}