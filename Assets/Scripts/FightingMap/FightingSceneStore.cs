using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class FightingSceneStore
{
    // Game Script
    public static CastSystem CastSystem;
    public static TurnBasedSystem TurnBasedSystem;

    // Map
    public static Tilemap tilemap;
    public static Tilemap cellsGrid;

    // Damageable items list
    public static Dictionary<Unit, GameObject> enemyList;
    public static Dictionary<Unit, GameObject> playerList;
    public static Dictionary<Vector3Int, GameObject> obstacleList;

    // UI
    public static InfoScrollView EnemiesScrollView;
    public static InfoScrollView PlayersScrollView;
}
