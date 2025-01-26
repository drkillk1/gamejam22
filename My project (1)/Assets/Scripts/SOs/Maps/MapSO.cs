using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapParms", menuName = "MapData")]
public class MapSO : ScriptableObject
{
    public int iterations = 10;
    public int walkLen = 10;
    public bool isRandom = true;


}
