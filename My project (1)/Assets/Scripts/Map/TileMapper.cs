using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

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


    [SerializeField]
    private TileBase wallSideRight, wallSideLeft, wallBottm, wallFull, 
    wallInnerCornerDownLeft, wallInnerCornerDownRight, 
    wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;

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

    internal void PaintWallTiles(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if(WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = wallTileType;
        }
        else if(WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = wallSideRight;
        }
        else if(WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = wallSideLeft;
        }
        else if(WallTypesHelper.wallBottm.Contains(typeAsInt))
        {
            tile = wallBottm;
        }
        else if(WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFull;
        }

        if(tile != null)
        {
            PaintTile(walls, tile, position);
        }
        // PaintTile(walls, wallTileType, position);


    }

    private void PaintTile(Tilemap tilemap, TileBase type, Vector2Int tile)
    {
        var tilepos = tilemap.WorldToCell((Vector3Int)tile);
        tilemap.SetTile(tilepos, type);
    }

    internal void PaintCornerWallTiles(Vector2Int pos, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;


        if(WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if(WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if(WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if(WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if(WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpRight;
        }
        else if(WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpLeft;
        }
        else if(WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        else if(WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
        {
            tile = wallBottm;
        }

        if(tile != null)
        {
            PaintTile(walls, tile, pos);
        }
    }

    

    public void Clear()
    {
        floor.ClearAllTiles();
        walls.ClearAllTiles();
    }

}
