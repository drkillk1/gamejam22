using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    [SerializeField]
    private GameObject playerPrefab; // Reference to the player prefab
    private GameObject spawnedPlayer; // Track the spawned player instance

    
     // Reference to the PrefabPlacer

    protected override void RunRandomWalk()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsList = RandomWalkMazeGenerator.BinarySpacePartitioning(new BoundsInt((Vector3Int)start, new Vector3Int(
            roomWidth, roomHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floorL = new HashSet<Vector2Int>();
        if (randomWalkRooms)
        {
            floorL = CreateRoomsRandomly(roomsList);
        }
        else
        {
            floorL = CreateSimpleRooms(roomsList);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var roomL in roomsList)
        {
            var roomCenter = (Vector2Int)Vector3Int.RoundToInt(roomL.center);
            roomCenters.Add(roomCenter);
            Debug.Log($"Room center added: {roomCenter}");
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floorL.UnionWith(corridors);

        // Paint the floor and create walls
        tileMapper.PaintFloor(floorL);
        WallGenerator.CreateWalls(floorL, tileMapper);

        ItemPlacementHelper placementHelper = new ItemPlacementHelper(floorL, corridors);
        if (placementHelper == null)
        {
            
            Debug.LogError("ItemPlacementHelper failed to initialize.");
        }

        PrefabPlacer prefabPlacer = GetComponent<PrefabPlacer>();

        // Pass the floor data to PrefabPlacer
        if (prefabPlacer != null)
        {
            Debug.Log("Placing prefabs in the generated maze...");
            prefabPlacer.SetItemPlacementHelper(placementHelper);
            prefabPlacer.PlacePrefabs(floorL, corridors); // Use the generated floor and corridor data
        }
        else
        {
            Debug.LogWarning("PrefabPlacer is not assigned in the BSPGenerator!");
        }

        MovePlayerToFirstRoom(roomsList);
    }

    private void MovePlayerToFirstRoom(List<BoundsInt> roomsList)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player object is not assigned! Please assign the player object in the Inspector.");
            return;
        }

        // Check if there are any rooms
        if (roomsList == null || roomsList.Count == 0)
        {
            Debug.LogWarning("No rooms available to move the player! Using fallback position.");
            playerPrefab.transform.position = new Vector3(roomWidth / 2, roomHeight / 2, 0); // Fallback to maze center
            return;
        }

        // Use the center of the first room as the spawn point
        BoundsInt firstRoom = roomsList[0];
        Vector3 newPlayerPosition = new Vector3(firstRoom.center.x, firstRoom.center.y, 0);
        playerPrefab.transform.position = newPlayerPosition;

        Debug.Log($"Player moved to first room center: {newPlayerPosition}");
        Debug.Log($"Player position after spawning: {playerPrefab.transform.position}");
    }





    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = Walk(mapParms, roomCenter);
            
            foreach (var pos in roomFloor)
            {
                if (pos.x >= (roomBounds.xMin + offset) && pos.x <= (roomBounds.xMax - offset) &&
                    pos.y >= (roomBounds.yMin + offset) && pos.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(pos);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var curRoomCenter = roomCenters[UnityEngine.Random.Range(0, roomCenters.Count)];

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(curRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(curRoomCenter, closest);
            curRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private Vector2Int FindClosestPointTo(Vector2Int curRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var pos in roomCenters)
        {
            float curDistance = Vector2.Distance(pos, curRoomCenter);
            if (curDistance < distance)
            {
                distance = curDistance;
                closest = pos; 
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int curRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var pos = curRoomCenter;
        corridor.Add(pos);
        while (pos.y != destination.y)
        {
            if (destination.y > pos.y)
            {
                pos += Vector2Int.up;
            }
            if (destination.y < pos.y)
            {
                pos += Vector2Int.down;
            }
            corridor.Add(pos);
        }
        while (pos.x != destination.x)
        {
            if (destination.x > pos.x)
            {
                pos += Vector2Int.right;
            }
            if (destination.x < pos.x)
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
        foreach (var roomL in roomsList)
        {
            for (int col = offset; col < roomL.size.x - offset; col++)
            {
                for (int row = offset; row < roomL.size.y - offset; row++)
                {
                    Vector2Int pos = (Vector2Int)roomL.min + new Vector2Int(col, row);
                    floorL.Add(pos);
                }
            }
        }
        return floorL;
    }
}
