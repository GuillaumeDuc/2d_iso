using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PhantomAI : EnemyAI
{
    /*
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
        Spell blackHole = unit.spellList[0].GetComponent<Spell>();
        Spell teleportation = unit.spellList[1].GetComponent<Spell>();
        // Get nearest player
        Unit nearestPlayer = getNearestPlayer(MoveSystem, obstacleList, unit.getEnemyTeam(), tilemap);
        bool isInRange = moveInRange(blackHole, nearestPlayer, MoveSystem, obstacleList, tilemap);
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
                tilemap,
                endTurn
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
        cast(spell, target, CastSystem, obstacleList, playerList, enemyList, tilemap, endTurn);
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
            Vector3Int maxPos = path.ElementAt(path.Count() - 1);
            KeyValuePair<Unit, GameObject> player = playerList.First(s => s.Key.position == maxPos);
            if (player.Key != null)
            {
                path.RemoveAt(path.Count() - 1);
            }
        }
        bool casted = false;
        // Try to cast on every cell from path 
        while (path.Any() && !casted)
        {
            unit.selectedSpellPos.Add(path[path.Count() - 1]);
            casted = CastSystem.castSpell(path[path.Count() - 1], spell, unit);
            path.RemoveAt(path.Count() - 1);
            unit.selectedSpellPos.Clear();
        }
    }*/
}
