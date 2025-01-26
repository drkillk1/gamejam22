using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapper : MonoBehaviour
{
    [SerializeField]
    private Tilemap floor;

    [SerializeField]
    private Tilemap walls;
    [SerializeField]
    private TileBase floorTileType;
    [SerializeField]
    private TileBase wallTileType;

    public void PaintFloor(IEnumerable<Vector2Int> floorPos)
    {
        PaintTiles(floorPos, floor, floorTileType);
    }

    public void PaintTiles(IEnumerable<Vector2Int> tiles, Tilemap tilemap, TileBase type)
    {
        foreach(var tile in tiles)
        {
            PaintTile(tilemap, type, tile);
        }
    }

    public void PaintWallTiles(Vector2Int tile)
    {
        PaintTile(walls, wallTileType, tile);
    }

    private void PaintTile(Tilemap tilemap, TileBase type, Vector2Int tile)
    {
        var tilepos = tilemap.WorldToCell((Vector3Int)tile);
        tilemap.SetTile(tilepos, type);
    }

    

    public void Clear()
    {
        floor.ClearAllTiles();
        walls.ClearAllTiles();
    }
}
