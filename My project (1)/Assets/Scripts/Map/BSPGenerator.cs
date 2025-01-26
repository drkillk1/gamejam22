using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class BSPGenerator : MazeFilller
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int roomWidth = 20, roomHeight = 20;
    [SerializeField]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;

    protected override void RunRandomWalk()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsList = RandomWalkMazeGenerator.BinarySpacePartitioning(new BoundsInt((Vector3Int)start, new Vector3Int(
            roomWidth, roomHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floorL = new HashSet<Vector2Int>();
        floorL = CreateSimpleRooms(roomsList);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach(var roomL in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(roomL.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floorL.UnionWith(corridors);

        tileMapper.PaintFloor(floorL);
        WallGenerator.CreateWalls(floorL, tileMapper);

    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var curRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];

        while(roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(curRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(curRoomCenter,closest);
            curRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private Vector2Int FindClosestPointTo(Vector2Int curRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closet = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach(var pos in roomCenters)
        {
            float curDistance = Vector2.Distance(pos, curRoomCenter);
            if(curDistance < distance)
            {
                distance = curDistance;
                closet = pos; 
            }
        }
        return closet;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int curRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var pos = curRoomCenter;
        corridor.Add(pos);
        while(pos.y != destination.y)
        {
            if(destination.y > pos.y)
            {
                pos += Vector2Int.up;
            }
            if(destination.y < pos.y)
            {
                pos += Vector2Int.down;
            }
            corridor.Add(pos);
        }
        while(pos.x != destination.x)
        {
            if(destination.x > pos.x)
            {
                pos += Vector2Int.right;
            }
            if(destination.x < pos.x)
            {
                pos += Vector2Int.left;
            }
            corridor.Add(pos);
        }
        return corridor;

    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floorL = new HashSet<Vector2Int>();
        foreach(var roomL in roomsList)
        {
            for(int col = offset; col < roomL.size.x - offset; col++)
            {
                for(int row = offset; row < roomL.size.y - offset; row++)
                {
                    Vector2Int pos = (Vector2Int)roomL.min + new Vector2Int(col, row);
                    floorL.Add(pos);
                }
            }
        }
        return floorL;
    }
}
