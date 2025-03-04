using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BSPGenerator : MazeFilller
{

    [SerializeField]
    private SpawnablePrefab[] roomConfigurations; // Array of SpawnablePrefab for room types
    [SerializeField]
    private float[] roomConfigurationRatios; // Corresponding ratios for each configuration
    private Dictionary<BoundsInt, HashSet<Vector2Int>> roomFloors = new Dictionary<BoundsInt, HashSet<Vector2Int>>();
    private Dictionary<BoundsInt, SpawnablePrefab> roomConfigurationsDict = new Dictionary<BoundsInt, SpawnablePrefab>();
    private List<BoundsInt> specialRooms = new List<BoundsInt>(); // Store special rooms
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
        roomFloors.Clear();
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

        if (prefabPlacer == null)
        {
            Debug.LogError("PrefabPlacer component is missing!");
            return;
        }

        // Assign special rooms (Player, Key, Boss)
        if (roomsList.Count < 3)
        {
            Debug.LogError("Not enough rooms for Player, Key, and Boss!");
            return;
        }

        prefabPlacer.SetItemPlacementHelper(placementHelper);
        prefabPlacer.ClearPast();
        
        AssignSpecialRooms(roomsList,prefabPlacer,corridors);

        AssignRoomConfigurations(roomsList.Skip(3).ToList());

        PlaceRoomContent(prefabPlacer, corridors);

    }

    private void AssignSpecialRooms(List<BoundsInt> roomsList, PrefabPlacer prefabPlacer, HashSet<Vector2Int> corridors) //ds3
    {
        if (roomsList.Count < 3)
        {
            Debug.LogError("Not enough rooms for Player, Key, and Boss!");
            return;
        }

        // Find the player room randomly
        int playerIndex = UnityEngine.Random.Range(0, roomsList.Count);
        BoundsInt playerRoom = roomsList[playerIndex];
        specialRooms.Add(playerRoom); // Add player room to special rooms

        // Find the furthest room from player for the boss
        BoundsInt bossRoom = roomsList
            .OrderByDescending(room => Vector2Int.Distance(Vector2Int.RoundToInt(playerRoom.center), Vector2Int.RoundToInt(room.center)))
            .First();
        specialRooms.Add(bossRoom); // Add boss room to special rooms

        // Find a key room that is reachable from the player room and before the boss room
        BoundsInt keyRoom = roomsList
            .Where(room => room != playerRoom && room != bossRoom)
            .OrderBy(room => Vector2Int.Distance(Vector2Int.RoundToInt(playerRoom.center), Vector2Int.RoundToInt(room.center)))
            .First();
        specialRooms.Add(keyRoom); // Add key room to special rooms

        // Move Player
        MovePlayerToRoom(playerRoom);

        // Assign Key Room
        if (roomFloors.ContainsKey(keyRoom))
        {
            prefabPlacer.spawnablePrefab = roomConfigurations[0];
            ItemPlacementHelper keyRoomPlacementHelper = new ItemPlacementHelper(roomFloors[keyRoom], corridors);
            prefabPlacer.SetItemPlacementHelper(keyRoomPlacementHelper);
            prefabPlacer.PlacePrefabs(roomFloors[keyRoom], corridors);
        }

        // Assign Boss Room
        LockBossRoom(bossRoom, prefabPlacer, corridors);

        Debug.Log($"Player Room: {playerRoom.center}, Key Room: {keyRoom.center}, Boss Room: {bossRoom.center}");
    }

    private void MovePlayerToRoom(BoundsInt room)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is not assigned!");
            return;
        }
        Vector3 playerPosition = new Vector3(room.center.x, room.center.y, 0);
        playerPrefab.transform.position = playerPosition;
        Debug.Log($"Player placed in room at: {playerPosition}");
    }

        private void LockBossRoom(BoundsInt bossRoom, PrefabPlacer prefabPlacer, HashSet<Vector2Int> corridors) //ds
    {
        if (!roomFloors.ContainsKey(bossRoom) || roomFloors[bossRoom].Count == 0)
        {
            Debug.LogError($"Boss room at {bossRoom.center} has no stored floor tiles!");
            return;
        }

        prefabPlacer.spawnablePrefab = roomConfigurations[1]; // Boss room configuration
        HashSet<Vector2Int> bossRoomTiles = roomFloors[bossRoom]; // Get the boss room's floor tiles

        // Create a new ItemPlacementHelper for the boss room
        ItemPlacementHelper bossRoomPlacementHelper = new ItemPlacementHelper(bossRoomTiles, corridors);
        prefabPlacer.SetItemPlacementHelper(bossRoomPlacementHelper);

        // Place prefabs only in the boss room's floor tiles
        prefabPlacer.PlacePrefabs(bossRoomTiles, corridors);

        Debug.Log("Boss Room locked.");
        Debug.Log($"Boss Room locked at {bossRoom.center} with {bossRoomTiles.Count} tiles.");
    }

    private void AssignRoomConfigurations(List<BoundsInt> unassignedRooms)//ds
    {
        if (roomConfigurationRatios.Length != roomConfigurations.Length)
        {
            Debug.LogError("Configuration ratios do not match room configurations!");
            return;
        }

        int remainingRooms = unassignedRooms.Count;
        int[] roomCounts = roomConfigurationRatios
            .Select(ratio => Mathf.RoundToInt(ratio * remainingRooms))
            .ToArray();

        int totalAssigned = roomCounts.Sum();
        while (totalAssigned > remainingRooms)
        {
            roomCounts[UnityEngine.Random.Range(2, roomCounts.Length)]--;
            totalAssigned--;
        }

        int roomIndex = 0;
        for (int i = 2; i < roomConfigurations.Length; i++) 
        {
            for (int j = 0; j < roomCounts[i]; j++)
            {
                if (roomIndex >= unassignedRooms.Count) break;

                var room = unassignedRooms[roomIndex];
                roomConfigurationsDict[room] = roomConfigurations[i]; // Store the assigned room configuration
                roomIndex++;
            }
        }
    }

    private void PlaceRoomContent(PrefabPlacer prefabPlacer, HashSet<Vector2Int> corridors)//ds2
    {
        foreach (var room in roomConfigurationsDict.Keys)
        {
            // Skip special rooms
            if (specialRooms.Contains(room))
            {
                Debug.Log($"Skipping special room at {room.center}.");
                continue;
            }

            if (!roomFloors.ContainsKey(room))
            {
                Debug.LogWarning($"Skipping room at {room.center} because it has no stored floor tiles.");
                continue;
            }

            HashSet<Vector2Int> roomFloorTiles = roomFloors[room];
            SpawnablePrefab roomConfig = roomConfigurationsDict[room];

            // Create a new ItemPlacementHelper for this room
            ItemPlacementHelper roomPlacementHelper = new ItemPlacementHelper(roomFloorTiles, corridors);
            prefabPlacer.SetItemPlacementHelper(roomPlacementHelper);

            // Set the room configuration and place prefabs
            prefabPlacer.spawnablePrefab = roomConfig;
            prefabPlacer.PlacePrefabs(roomFloorTiles, corridors);
        }
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
        // roomFloors.Clear(); // Ensure room tracking is reset

        foreach (var roomBounds in roomsList)
        {
            HashSet<Vector2Int> validRoomTiles = new HashSet<Vector2Int>();

            // Convert the center to Vector2Int
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = Walk(mapParms, roomCenter);

            foreach (var pos in roomFloor)
            {
                if (pos.x >= (roomBounds.xMin + offset) && pos.x <= (roomBounds.xMax - offset) &&
                    pos.y >= (roomBounds.yMin + offset) && pos.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(pos);
                    validRoomTiles.Add(pos);
                }
            }

            if (validRoomTiles.Count > 0)
            {
                roomFloors[roomBounds] = validRoomTiles;
            }
            else
            {
                Debug.LogWarning($"Room at {roomBounds.center} has no valid floor tiles!");
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

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList) //ds
    {
        HashSet<Vector2Int> floorL = new HashSet<Vector2Int>();
        roomFloors.Clear(); // Ensure room tracking is reset

        foreach (var roomL in roomsList)
        {
            HashSet<Vector2Int> roomTiles = new HashSet<Vector2Int>();

            for (int col = offset; col < roomL.size.x - offset; col++)
            {
                for (int row = offset; row < roomL.size.y - offset; row++)
                {
                    Vector2Int pos = (Vector2Int)roomL.min + new Vector2Int(col, row);
                    floorL.Add(pos);
                    roomTiles.Add(pos);
                }
            }

            if (roomTiles.Count > 0)
            {
                roomFloors[roomL] = roomTiles; // Store the room's floor tiles
                Debug.Log($"Room at {roomL.center} stored with {roomTiles.Count} tiles.");
                PrintRoomTiles(roomL, roomTiles); // Print the room's tiles
            }
            else
            {
                Debug.LogError($"Failed to store tiles for room at {roomL.center}.");
            }
        }

        return floorL;
    }

    private void PrintRoomTiles(BoundsInt room, HashSet<Vector2Int> roomTiles)
    {
        Debug.Log($"Room at {room.center} has the following tiles:");
        foreach (var tile in roomTiles)
        {
            Debug.Log($"Tile: {tile}");
        }
    }
}
