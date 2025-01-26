using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeFilller : AbstractMazeGenerator
{
    [SerializeField]
    protected MapSO mapParms;

    


    protected override void RunRandomWalk()
    {
        HashSet<Vector2Int> floorPositions = Walk(mapParms,start);
        Debug.Log($"Floor positions count: {floorPositions.Count}");
        tileMapper.PaintFloor(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tileMapper);
        
        
        ///new
        // Identify the room floor (no corridors, if applicable)
        HashSet<Vector2Int> noCorridorPositions = new HashSet<Vector2Int>(floorPositions); // Adjust for your corridor logic if needed
        Debug.Log($"No-corridor positions count: {noCorridorPositions.Count}");

        // Initialize the ItemPlacementHelper
        ItemPlacementHelper placementHelper = new ItemPlacementHelper(floorPositions, noCorridorPositions);
        if (placementHelper == null)
        {
            
            Debug.LogError("ItemPlacementHelper failed to initialize.");
        }

        // Place prefabs (items and enemies)
        PrefabPlacer prefabPlacer = GetComponent<PrefabPlacer>();
        
        if (prefabPlacer != null)
        {
            Debug.LogError("ItemPlacementHelper failed to initialize.");
            prefabPlacer.SetItemPlacementHelper(placementHelper);
            prefabPlacer.PlacePrefabs(floorPositions, noCorridorPositions);
        }
        else
        {
            Debug.LogError("PrefabPlacer component is missing from this GameObject.");
        }
        //new
            
    }

    

    protected HashSet<Vector2Int> Walk(MapSO parms, Vector2Int pos)
    {
        var curPos = pos;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for(int i = 0; i < parms.iterations; i++)
        {
            var path = RandomWalkMazeGenerator.SimpleRandomWalk(curPos, parms.walkLen);
            floorPositions.UnionWith(path);
            if(parms.isRandom)
            {
                curPos = floorPositions.ElementAt(Random.Range(0,floorPositions.Count()));
            }
        }
        return floorPositions;
    }
}
