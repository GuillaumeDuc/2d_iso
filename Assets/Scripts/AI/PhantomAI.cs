using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PhantomAI : EnemyAI
{

    private RangeUtils RangeUtils;

    private void Start()
    {
        // Instantiate Utils to get area & calculations
        RangeUtils = new RangeUtils();
    }

    public override void play(
        MoveSystem MoveSystem, 
        CastSystem CastSystem, 
        Dictionary<Vector3Int, GameObject> obstacleList, 
        Dictionary<Unit, GameObject> playerList, 
        Dictionary<Unit, GameObject> enemyList, 
        Tilemap tilemap,
        Action endTurn
        )
    {

        // Choose spell
        Spell blackHole = unit.spellList[0];
        Spell teleportation = unit.spellList[1];
        // Get nearest player
        Unit nearestPlayer = getNearestPlayer(MoveSystem, obstacleList, playerList, tilemap);
        bool isInRange = moveInRange(blackHole, nearestPlayer, MoveSystem, obstacleList, playerList, tilemap);
        // If is in range
        if (isInRange)
        {
            cast(
                blackHole,
                nearestPlayer.position,
                CastSystem,
                obstacleList,
                playerList,
                enemyList,
                tilemap
            );
            endTurn();
        }
        // If is not in range tp in then try casting again
        if (!isInRange)
        {
            // Cast Teleportation
            StartCoroutine(castToNearestPlayer(
                teleportation,
                nearestPlayer.position,
                CastSystem,
                obstacleList,
                playerList,
                enemyList,
                tilemap
            ));
            // Then cast attack
            StartCoroutine(castWithDelay(
                blackHole,
                nearestPlayer.position,
                CastSystem,
                obstacleList,
                playerList,
                enemyList,
                tilemap,
                endTurn
            ));
        }
    }

    public IEnumerator castWithDelay(
        Spell spell,
        Vector3Int target,
        CastSystem CastSystem,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Tilemap tilemap,
        Action endTurn
        )
    {
        yield return new WaitForSeconds(1.7f);
        cast(spell, target, CastSystem, obstacleList, playerList, enemyList, tilemap);
        endTurn();
    }


    public IEnumerator castToNearestPlayer(
        Spell spell,
        Vector3Int target,
        CastSystem CastSystem,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Tilemap tilemap
        )
    {
        yield return new WaitForSeconds(1.5f);
        List<Vector3Int> path = RangeUtils.getLine(unit.position, target);
        // Remove where player is standing
        if (path.Any())
        {
            path.RemoveAt(path.Count() - 1);
        }
        bool casted = false;
        // Try to cast on every cell from path 
        while (path.Any() && !casted)
        {
            spell.spellPos.Add(path[path.Count() - 1]);
            if (spell.canCast(unit, path[path.Count() - 1], obstacleList, tilemap))
            {
                casted = true;
                CastSystem.castSpell(spell, unit, playerList, enemyList, obstacleList, tilemap);
            }
            path.RemoveAt(path.Count() - 1);
            spell.spellPos.Clear();
        }
    }
}
