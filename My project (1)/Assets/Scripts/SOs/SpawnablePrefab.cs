using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PrefabConfig", menuName = "Dungeon/PrefabConfig", order = 1)]
public class SpawnablePrefab: ScriptableObject
{
    [System.Serializable]
    public class PrefabEntry
    {
        public GameObject prefab;
        public int quantity;
        public Vector2Int size; // Optional: specify size for larger items.
    }

    public PrefabEntry[] enemies;
    public PrefabEntry[] items;
}

