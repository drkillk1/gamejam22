using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomWalkMazeGenerator 
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int start, int walkLen)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        path.Add(start);
        var prevPos = start;
        
        for(int i = 0; i < walkLen; i++)
        {
            var newPos = prevPos + CardDirections.GetRandDir();
            path.Add(newPos);
            prevPos = newPos;
        }
        
        return path;
    }
}


public static class CardDirections
{
    public static List<Vector2Int> cardDirectionList = new List<Vector2Int>
    {
        new Vector2Int(1, 0), //Right
        new Vector2Int(0, 1), //Up
        new Vector2Int(-1, 0), //Left
        new Vector2Int(0, -1) //Down
    };

    public static Vector2Int GetRandDir()
    {
        return cardDirectionList[Random.Range(0, cardDirectionList.Count)];
    }
}