using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPlacer : MonoBehaviour
{
    [SerializeField]
    private SpawnablePrefab spawnablePrefab;
    [SerializeField]
    private ItemPlacementHelper itemPlacementHelper;

    public void PlacePrefabs(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> noCorridor)
    {
        PlacePrefabsOfType(spawnablePrefab.enemies, roomFloor, noCorridor);
        PlacePrefabsOfType(spawnablePrefab.items, roomFloor, noCorridor);
    }

    private void PlacePrefabsOfType(SpawnablePrefab.PrefabEntry[] entries, HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> noCorridor)
    {
        foreach (var entry in entries)
        {
            for (int i = 0; i < entry.quantity; i++)
            {
                Vector2? position = itemPlacementHelper.GetItemPlacementPosition(
                    ItemPlacementHelper.PlacementType.OpenSpace, 100, entry.size, false);

                if (position.HasValue)
                {
                    Debug.Log($"Spawning {entry.prefab.name} at {position.Value}");
                    Vector3 worldPosition = new Vector3(position.Value.x, position.Value.y, 0);
                    Instantiate(entry.prefab, worldPosition, Quaternion.identity);
                }
                else{
                    Debug.LogWarning($"Failed to find a position for {entry.prefab.name}");
                }
            }
        }
    }
}
