using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPlacer : MonoBehaviour
{
    
    [SerializeField] public SpawnablePrefab spawnablePrefab; // ScriptableObject containing enemies and items
    private ItemPlacementHelper itemPlacementHelper; // Helper for finding placement positions
    private List<GameObject> spawnedEnemies = new List<GameObject>(); // Track spawned enemies
    private List<GameObject> spawnedItems = new List<GameObject>();

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
        // DeleteSpawnedEnemies();
        // DeleteSpawnedItems();

        // Attempt to place enemies and items
        PlacePrefabsOfType(spawnablePrefab.enemies, roomFloor, noCorridor, "Enemies", true);
        PlacePrefabsOfType(spawnablePrefab.items, roomFloor, noCorridor, "Items", true);
    }

    private void PlacePrefabsOfType(SpawnablePrefab.PrefabEntry[] entries, HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> noCorridor, string typeName, bool trackSpawnedObjects)
    {
        if (entries == null || itemPlacementHelper == null) return;

        foreach (var entry in entries)
        {
            if (entry.prefab == null) continue;

            Debug.Log($"Placing {entry.quantity} instances of {entry.prefab.name}");

            for (int i = 0; i < entry.quantity; i++)
            {
                Vector2? position = itemPlacementHelper.GetItemPlacementPosition(
                    ItemPlacementHelper.PlacementType.OpenSpace, 100, entry.size, false);

                if (!position.HasValue)
                {
                    Debug.LogWarning($"No valid position found for {entry.prefab.name}. Trying near-wall placement.");
                    position = itemPlacementHelper.GetItemPlacementPosition(
                        ItemPlacementHelper.PlacementType.NearWall, 100, entry.size, false);
                }

                if (position.HasValue)
                {
                    Vector3 worldPosition = new Vector3(position.Value.x, position.Value.y, 0);
                    GameObject instance = Instantiate(entry.prefab, worldPosition, Quaternion.identity);
                    Debug.Log($"Spawned {entry.prefab.name} at {worldPosition}");

                    if (trackSpawnedObjects && typeName == "Enemies") spawnedEnemies.Add(instance);
                    if (trackSpawnedObjects && typeName == "Items") spawnedItems.Add(instance);
                }
                else
                {
                    Debug.LogWarning($"Failed to place {entry.prefab.name} after multiple attempts.");
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

    private void DeleteSpawnedItems()
    {
        if (spawnedItems.Count > 0)
        {
            Debug.Log($"Deleting {spawnedItems.Count} previously spawned items.");
            foreach (var item in spawnedItems)
            {
                if (item != null)
                {
                    // Use DestroyImmediate in the Editor and Destroy during Play mode
                    if (Application.isPlaying)
                    {
                        Destroy(item);
                    }
                    else
                    {
                        DestroyImmediate(item);
                    }
                }
            }
            spawnedItems.Clear();
        }
        else
        {
            Debug.Log("No previously spawned items to delete.");
        }
    }

    public void ClearPast()
    {
        DeleteSpawnedEnemies();
        DeleteSpawnedItems();
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

    internal object GetItemPlacementHelper()
    {
        return itemPlacementHelper;
    }
}
