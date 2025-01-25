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
    private TileBase tileType;

    public void PaintFloor(IEnumerable<Vector2Int> floorPos)
    {
        PaintTile(floorPos, floor, tileType);
    }

    public void PaintTile(IEnumerable<Vector2Int> tiles, Tilemap tilemap, TileBase type)
    {
        foreach(var tile in tiles)
        {
            Paint(tilemap, type, tile);
        }
    }

    private void Paint(Tilemap tilemap, TileBase type, Vector2Int tile)
    {
        var tilepos = tilemap.WorldToCell((Vector3Int)tile);
        tilemap.SetTile(tilepos, type);
    }
}
