using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAI : MonoBehaviour
{
    public Unit unit;

    [HideInInspector]
    public bool endTurn = false, casting = false;
    void Update()
    {
        if (unit.isPlaying && !casting)
        {
            play(
                FightingSceneStore.MoveSystem,
                FightingSceneStore.CastSystem,
                FightingSceneStore.obstacleList,
                FightingSceneStore.playerList,
                FightingSceneStore.enemyList,
                FightingSceneStore.tilemap
            );
            casting = true;
        }

        if (endTurn)
        {
            endTurn = false;
            casting = false;
            FightingSceneStore.TurnBasedSystem.onClickEndTurn();
        }
    }
    public virtual void play(
        MoveSystem MoveSystem,
        CastSystem CastSystem,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Tilemap tilemap
        )
    {
        // Choose spell
        GameObject spellGO = unit.spellList[0];
        Spell spell = spellGO.GetComponent<Spell>();
        // Get nearest player
        Unit nearestPlayer = getNearestPlayer(MoveSystem, obstacleList, unit.getEnemyTeam(), tilemap);
        if (nearestPlayer != null)
        {
            bool isInRange = moveInRange(spell, nearestPlayer, MoveSystem, obstacleList, tilemap);
            // If is in range
            if (isInRange)
            {
                StartCoroutine(cast(
                    spell,
                    nearestPlayer.position,
                    CastSystem,
                    obstacleList,
                    playerList,
                    enemyList,
                    tilemap
                ));
            }
            else
            {
                endTurn = true;
            }
        }
    }

    public bool moveInRange(
        Spell spell,
        Unit nearestPlayer,
        MoveSystem MoveSystem,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        // Get path to nearest player
        List<Square> path = MoveSystem.getPathCharacter(
                new Square(unit.position),
                new Square(nearestPlayer.position),
                obstacleList,
                tilemap
                );
        // If no path is found, cannot move in range
        if (!path.Any())
        {
            return false;
        }
        // Remove cell where player is standing
        path.RemoveAt(0);
        // Get spell
        bool canCast = spell.canCast(unit, spell.getRange(unit, obstacleList, tilemap), nearestPlayer.position, obstacleList, tilemap);
        // Move while can't cast spell
        while (unit.currentMovementPoint > 0 && !canCast && path.Any())
        {
            // Move one square
            Square square = path[0];
            path.RemoveAt(0);
            MoveSystem.moveOneSquare(square, unit, gameObject, tilemap);
            // Check if can cast
            canCast = spell.canCast(unit, spell.getRange(unit, obstacleList, tilemap), nearestPlayer.position, obstacleList, tilemap);
            if (canCast)
            {
                return true;
            }
        }
        return canCast;
    }

    public IEnumerator cast(
        Spell spell,
        Vector3Int target,
        CastSystem CastSystem,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Tilemap tilemap
    )
    {
        yield return new WaitForSeconds(1f);
        // Always in range
        bool canCast = true;
        while (canCast)
        {
            for (int i = 0; i < spell.clickNb; i++)
            {
                unit.selectedSpellPos.Add(target);
            }
            // Casting on player
            canCast = CastSystem.castSpell(target, spell, unit);
            unit.selectedSpellPos.Clear();
            yield return new WaitForSeconds(0.5f);
        }
        endTurn = true;
    }

    public Unit getNearestPlayer(
        MoveSystem MoveSystem,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Dictionary<Unit, GameObject> playerList,
        Tilemap tilemap
        )
    {
        Unit currentUnit = null;
        int lowest = 100000;
        foreach (var p in playerList)
        {
            List<Square> path = MoveSystem.getPathCharacter(
                new Square(unit.position),
                new Square(p.Key.position),
                obstacleList,
                tilemap
                );
            if (path.Count() < lowest)
            {
                lowest = path.Count();
                currentUnit = p.Key;
            }
        }
        return currentUnit;
    }
}
