using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    private static List<Vector2Int> neighbours4Directions = new List<Vector2Int>
    {
        new Vector2Int(0, 1), //Up
        new Vector2Int(1, 0), //Right
        new Vector2Int(0, -1), //Down
        new Vector2Int(-1, 0) //Left
    };

    private static List<Vector2Int> neighbours8Directions = new List<Vector2Int>
    {
        new Vector2Int(0, 1), //Up
        new Vector2Int(1, 0), //Right
        new Vector2Int(0, -1), //Down
        new Vector2Int(-1, 0), //Left
        new Vector2Int(1, 1), //Digonal
        new Vector2Int(1, -1), //Digonal
        new Vector2Int(-1, 1), //Digonal
        new Vector2Int(-1, -1) //Digonal
        
    };

    List<Vector2Int> graph;

    public Graph(IEnumerable<Vector2Int> verticies)
    {
        graph = new List<Vector2Int>(verticies);
    }

    public List<Vector2Int> GetNeighbours4Directions(Vector2Int startPosition)
    {
        return GetNeighbours(startPosition, neighbours4Directions);
    }

    public List<Vector2Int> GetNeighbours8Directions(Vector2Int startPosition)
    {
        return GetNeighbours(startPosition, neighbours8Directions);
    }

    private List<Vector2Int> GetNeighbours(Vector2Int startPosition, List<Vector2Int> neighboursOffsetList)
    {
        List<Vector2Int>  neighbours = new List<Vector2Int>();
        foreach(var neighbourDirection in neighboursOffsetList)
        {
            Vector2Int potentialNeighbor = startPosition + neighbourDirection;
            if(graph.Contains(potentialNeighbor))
            {
                neighbours.Add(potentialNeighbor);
            }
        }
        return neighbours;
    }
}
