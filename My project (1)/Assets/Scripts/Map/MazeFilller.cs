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
        tileMapper.PaintFloor(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tileMapper);
        //foreach(var pos in floorPositions)
        //{
        //    Debug.Log(pos);
        //}
        
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
