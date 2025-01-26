using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMazeGenerator : MonoBehaviour
{
    [SerializeField]
    protected TileMapper tileMapper;
    [SerializeField]
    protected Vector2Int start = Vector2Int.zero;

    public void GenerateMaze()
    {
        tileMapper.Clear();
        RunRandomWalk();
    }
    protected abstract void RunRandomWalk();
}
