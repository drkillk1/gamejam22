using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public class CorridorFirstMazeGen : MazeFilller
{
    [SerializeField]
    private int corridorLen = 10;
    [SerializeField]
    private int corridorcount = 5;
    [SerializeField]
    [Range(0.1f, 1)]
    private float roomPercent = 0.8f;

    protected override void RunRandomWalk()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRoomPositions(potentialRoomPositions);
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        tileMapper.PaintFloor(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tileMapper);



    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach(var position in floorPositions)
        {
            int numNeighbors = 0;
            foreach(var dir in CardDirections.cardDirectionList)
            {
                if(floorPositions.Contains(position + dir))
                {
                    numNeighbors++;
                }
                if(numNeighbors == 1)
                {
                    deadEnds.Add(position);
                }
            }
        }
        return deadEnds;
    }

    private void CreateRoomAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach(var pos in deadEnds)
        {
            if(roomFloors.Contains(pos) == false)
            {
                var roomFloor = Walk(mapParms, pos);
                roomFloors.UnionWith(roomFloor);
            }
        }
    }

    private HashSet<Vector2Int> CreateRoomPositions(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int numRooms = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(numRooms).ToList();

        foreach(var roomPos in roomsToCreate)
        {
            var roomFloor = Walk(mapParms, roomPos);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var curPos = start;

        for(int i = 0; i < corridorcount; i++)
        {
            var corridor = RandomWalkMazeGenerator.RandomWalkCorridor(curPos,corridorLen);
            curPos = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(curPos);
            floorPositions.UnionWith(corridor);
        }

    }
}
