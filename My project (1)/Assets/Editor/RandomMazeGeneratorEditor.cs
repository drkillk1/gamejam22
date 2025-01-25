using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AbstractMazeGenerator), true)]
public class RandomMazeGeneratorEditor : Editor
{
    AbstractMazeGenerator generator;

    private void Awake()
    {
        generator = (AbstractMazeGenerator)target;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate Maze"))
        {
            generator.GenerateMaze();
        }
    }
}
