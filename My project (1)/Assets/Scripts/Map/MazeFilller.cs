using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeFilller : AbstractMazeGenerator
{
    [Header("Base Parms")]
    [SerializeField]
    int walkLen = 10;
    [SerializeField]
    int iterations = 10;
    [SerializeField]
    bool isRandom = true;

    protected override void RunRandomWalk()
    {
        HashSet<Vector2Int> floorPositions = Walk();
        tileMapper.PaintFloor(floorPositions);
        //foreach(var pos in floorPositions)
        //{
        //    Debug.Log(pos);
        //}
        
    }

    protected HashSet<Vector2Int> Walk()
    {
        var curPos = start;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for(int i = 0; i < iterations; i++)
        {
            var path = RandomWalkMazeGenerator.SimpleRandomWalk(curPos, walkLen);
            floorPositions.UnionWith(path);
            if(isRandom)
            {
                curPos = floorPositions.ElementAt(Random.Range(0,floorPositions.Count()));
            }
        }
        return floorPositions;
    }
}
