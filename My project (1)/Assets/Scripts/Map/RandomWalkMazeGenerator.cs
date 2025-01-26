using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

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

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int start, int corridorLen)
    {
        List<Vector2Int> corridorPath = new List<Vector2Int>();
        var direction = CardDirections.GetRandDir();
        Vector2Int curPos = start;
        corridorPath.Add(curPos);
        for(int i = 0; i < corridorLen; i++)
        {
            curPos += direction;
            corridorPath.Add(curPos);
        }
        return corridorPath;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minwidth, int minheight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);

        while(roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if(room.size.y >= minheight && room.size.x >= minwidth)
            {
                if(Random.value < 0.5f)
                {
                    if(room.size.y >= minheight * 2)
                    {
                        SplitHorizontally(minwidth, minheight, roomsQueue, room);
                    }
                    else if(room.size.x >= minwidth * 2)
                    {
                        SplitVertically(minwidth, minheight, roomsQueue, room);
                    }
                    else if(room.size.y >= minheight && room.size.x >= minwidth)
                    {
                        roomsList.Add(room);
                    }
                }
                else{
                    if(room.size.x >= minwidth * 2)
                    {
                        SplitVertically(minwidth, minheight, roomsQueue, room);
                    }
                    else if(room.size.y >= minheight * 2)
                    {
                        SplitHorizontally(minwidth, minheight, roomsQueue, room);
                    }
                    else if(room.size.y >= minheight && room.size.x >= minwidth)
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }
        return roomsList;
    }

    private static void SplitVertically(int minwidth, int minheight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minwidth, int minheight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y+ySplit, room.min.z), new Vector3Int(room.size.x, room.size.y-ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
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