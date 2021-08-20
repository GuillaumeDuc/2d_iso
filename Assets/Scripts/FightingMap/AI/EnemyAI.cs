using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAI : MonoBehaviour
{
    protected Unit unit;

    protected bool endTurn = false, casting = false, move = true;

    protected Spell spell;
    protected Unit nearestPlayer;

    void Start()
    {
        unit = gameObject.GetComponent<Unit>();
        // Choose spell
        GameObject spellGO = unit.spellList[0];
        spell = spellGO.GetComponent<Spell>();
    }

    protected virtual void Update()
    {
        if (!FightingSceneStore.TurnBasedSystem.gameOver)
        {
            if (unit.isPlaying)
            {
                // Move player
                if (move)
                {
                    // Get nearest player
                    nearestPlayer = getNearestPlayer(
                        FightingSceneStore.MoveSystem,
                        FightingSceneStore.obstacleList,
                        unit.getEnemyTeam(),
                        FightingSceneStore.tilemap
                        );
                    // Move
                    if (nearestPlayer != null)
                    {
                        bool isInRange = moveInRange(
                            spell,
                            nearestPlayer,
                            FightingSceneStore.MoveSystem,
                            FightingSceneStore.obstacleList,
                            FightingSceneStore.tilemap
                            );
                        // Is not in range
                        if (!isInRange)
                        {
                            endTurn = true;
                        }
                    }
                    else
                    {
                        endTurn = true;
                    }
                    move = false;
                    casting = true;
                }

                // Cast only when finished moving
                if (casting && !move && !FightingSceneStore.MoveSystem.isMoving)
                {
                    StartCoroutine(cast(
                        spell,
                        nearestPlayer.position,
                        FightingSceneStore.CastSystem,
                        FightingSceneStore.obstacleList,
                        FightingSceneStore.playerList,
                        FightingSceneStore.enemyList,
                        FightingSceneStore.tilemap
                    ));
                    casting = false;
                }

                //End turn
                if (endTurn)
                {
                    endTurn = false;
                    move = true;
                    casting = true;
                    FightingSceneStore.TurnBasedSystem.onClickEndTurn();
                }
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
        if (nearestPlayer == null)
        {
            return false;
        }
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
        yield return new WaitForSeconds(0.1f);
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
