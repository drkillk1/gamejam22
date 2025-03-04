using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public BoundsInt Bounds { get; set; } // Holds room bounds
    public RoomType Type { get; set; } = RoomType.None; // Type of the room (Start, Key, Boss, etc.)
    public SpawnablePrefab Configuration { get; set; } // Configuration assigned to this room
}

public enum RoomType
{
    None,
    StartRoom,
    KeyRoom,
    BossRoom,
    EnemyRoom,
    ItemRoom
}
