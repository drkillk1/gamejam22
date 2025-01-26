using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TileMapper tileMapper)
    {
        var wallPositions = FindWalls(floorPositions, CardDirections.cardDirectionList);
        foreach(var wallPos in wallPositions)
        {
            tileMapper.PaintWallTiles(wallPos);
        }
    }

    private static HashSet<Vector2Int> FindWalls(HashSet<Vector2Int> floorPositions, List<Vector2Int> cardDirectionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach(var pos in floorPositions)
        {
            foreach(var direction in cardDirectionList)
            {
                var neighbor = pos +direction;
                if(floorPositions.Contains(neighbor) == false)
                {
                    wallPositions.Add(neighbor);
                }
            }
        }
        return wallPositions;
    } 
}
