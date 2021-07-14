using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class FightingSceneStore
{
    public static Tilemap tilemap;
    public static Tilemap cellsGrid;
    public static Dictionary<Unit, GameObject> enemyList;
    public static Dictionary<Unit, GameObject> playerList;

    public static Dictionary<Vector3Int, GameObject> obstacleList;

    public static InfoScrollView EnemiesScrollView;
    public static InfoScrollView PlayersScrollView;

    public static SpellList SpellList;
    public static SpellInstantiateList SpellInstantiateList;
    public static SpellEffectList SpellEffectList;
    public static StatusList StatusList;
    public static TileList TileList;
}
