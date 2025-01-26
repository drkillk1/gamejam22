using System.Collections.Generic;
using UnityEngine;

public class PrefabPlacer : MonoBehaviour
{
    
    [SerializeField] private SpawnablePrefab spawnablePrefab; // ScriptableObject containing enemies and items
    private ItemPlacementHelper itemPlacementHelper; // Helper for finding placement positions
    private List<GameObject> spawnedEnemies = new List<GameObject>(); // Track spawned enemies

    /// <summary>
    /// Places prefabs (items and enemies) on the generated maze.
    /// </summary>
    /// <param name="roomFloor">The set of room floor positions (all walkable tiles).</param>
    /// <param name="noCorridor">The set of room positions excluding corridors.</param>
    public void PlacePrefabs(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> noCorridor)
    {
        Debug.Log($"Starting prefab placement. Room floor count: {roomFloor.Count}, No-corridor count: {noCorridor.Count}");

        // Ensure SpawnablePrefab is assigned
        if (spawnablePrefab == null)
        {
            Debug.LogError("SpawnablePrefab is null. Ensure it is assigned in the Inspector.");
            return;
        }

        // Check if enemies and items arrays are properly assigned
        if (spawnablePrefab.enemies == null)
        {
            Debug.LogError("Enemies array in SpawnablePrefab is null.");
            return;
        }

        if (spawnablePrefab.items == null)
        {
            Debug.LogError("Items array in SpawnablePrefab is null.");
            return;
        }

        // Ensure ItemPlacementHelper is set
        if (itemPlacementHelper == null)
        {
            Debug.LogError("ItemPlacementHelper is null. Cannot place prefabs.");
            return;
        }

        // Delete previously spawned enemies
        DeleteSpawnedEnemies();

        // Attempt to place enemies and items
        PlacePrefabsOfType(spawnablePrefab.enemies, roomFloor, noCorridor, "Enemies", true);
        PlacePrefabsOfType(spawnablePrefab.items, roomFloor, noCorridor, "Items", false);
    }

    /// <summary>
    /// Places a specific type of prefab (either enemies or items).
    /// </summary>
    /// <param name="entries">Array of prefab entries to place.</param>
    /// <param name="roomFloor">The set of room floor positions (all walkable tiles).</param>
    /// <param name="noCorridor">The set of room positions excluding corridors.</param>
    /// <param name="typeName">The type name for debug logs (e.g., "Enemies" or "Items").</param>
    /// <param name="trackSpawnedObjects">Whether to track the spawned objects for later deletion.</param>
    private void PlacePrefabsOfType(SpawnablePrefab.PrefabEntry[] entries, HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> noCorridor, string typeName, bool trackSpawnedObjects)
    {
        if (entries == null)
        {
            Debug.LogError($"{typeName} entries array is null.");
            return;
        }

        foreach (var entry in entries)
        {
            if (entry.prefab == null)
            {
                Debug.LogError($"Prefab is null in one of the {typeName} entries. Skipping this entry.");
                continue;
            }

            Debug.Log($"Placing {entry.quantity} instances of {entry.prefab.name}.");

            for (int i = 0; i < entry.quantity; i++)
            {
                Vector2? position = itemPlacementHelper.GetItemPlacementPosition(
                    ItemPlacementHelper.PlacementType.OpenSpace, 100, entry.size, false);

                if (position.HasValue)
                {
                    Vector3 worldPosition = new Vector3(position.Value.x, position.Value.y, 0);
                    GameObject instance = Instantiate(entry.prefab, worldPosition, Quaternion.identity);
                    Debug.Log($"Spawned {entry.prefab.name} at position {worldPosition}.");

                    // Track the spawned enemy for later deletion
                    if (trackSpawnedObjects)
                    {
                        spawnedEnemies.Add(instance);
                    }
                }
                else
                {
                    Debug.LogWarning($"No valid position found for {entry.prefab.name}. Skipping this instance.");
                }
            }
        }
    }

    /// <summary>
    /// Deletes all previously spawned enemies.
    /// </summary>
    private void DeleteSpawnedEnemies()
    {
        if (spawnedEnemies.Count > 0)
        {
            Debug.Log($"Deleting {spawnedEnemies.Count} previously spawned enemies.");
            foreach (var enemy in spawnedEnemies)
            {
                if (enemy != null)
                {
                    // Use DestroyImmediate in the Editor and Destroy during Play mode
                    if (Application.isPlaying)
                    {
                        Destroy(enemy);
                    }
                    else
                    {
                        DestroyImmediate(enemy);
                    }
                }
            }
            spawnedEnemies.Clear();
        }
        else
        {
            Debug.Log("No previously spawned enemies to delete.");
        }
    }


    /// <summary>
    /// Sets the ItemPlacementHelper instance.
    /// </summary>
    /// <param name="helper">The ItemPlacementHelper to set.</param>
    public void SetItemPlacementHelper(ItemPlacementHelper helper)
    {
        if (helper == null)
        {
            Debug.LogError("Failed to set ItemPlacementHelper. The provided helper is null.");
            return;
        }

        itemPlacementHelper = helper;
        Debug.Log("ItemPlacementHelper successfully set.");
    }
}
