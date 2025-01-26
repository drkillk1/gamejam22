using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TileMapper tileMapper)
    {
        var basicwallPositions = FindWalls(floorPositions, CardDirections.cardDirectionList);
        var cornerWallPositions = FindWalls(floorPositions, CardDirections.diagonalDirectionList);
        CreateBasicWalls(tileMapper, basicwallPositions,floorPositions);
        CreateCornerWalls(tileMapper,basicwallPositions,floorPositions);
    }

    private static void CreateBasicWalls(TileMapper tileMapper, HashSet<Vector2Int> basicwallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var pos in basicwallPositions)
        {
            string neighborsBinaryType = "";
            foreach(var direction in CardDirections.cardDirectionList)
            {
                var neighborPos = pos + direction;
                if(floorPositions.Contains(neighborPos))
                {
                    neighborsBinaryType += "1";
                }
                else
                {
                    neighborsBinaryType += "0";
                }
            }
            tileMapper.PaintWallTiles(pos, neighborsBinaryType);
        }
    }

    private static void CreateCornerWalls(TileMapper tileMapper, HashSet<Vector2Int> cornerwallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var pos in cornerwallPositions)
        {
            string neighborsBinaryType = "";
            foreach(var direction in CardDirections.diagonalDirectionList)
            {
                var neighborPos = pos + direction;
                if(floorPositions.Contains(neighborPos))
                {
                    neighborsBinaryType += "1";
                }
                else
                {
                    neighborsBinaryType += "0";
                }
            }
            tileMapper.PaintCornerWallTiles(pos, neighborsBinaryType);
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
