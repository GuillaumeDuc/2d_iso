using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PhantomAI : EnemyAI
{
    protected bool isInRange = false;
    Spell blackHole, teleportation;
    void Start()
    {
        unit = gameObject.GetComponent<Unit>();
        // Choose spell
        GameObject blackholeGO = unit.spellList[0];
        blackHole = blackholeGO.GetComponent<Spell>();

        GameObject teleportationGO = unit.spellList[1];
        teleportation = teleportationGO.GetComponent<Spell>();
    }

    protected override void Update()
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
                        isInRange = moveInRange(
                            blackHole,
                            nearestPlayer,
                            FightingSceneStore.MoveSystem,
                            FightingSceneStore.obstacleList,
                            FightingSceneStore.tilemap
                        );
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
                    // If phantom is not in range, cast tp
                    if (!isInRange)
                    {
                        castToNearestPlayer(
                            teleportation,
                            nearestPlayer.position,
                            FightingSceneStore.CastSystem,
                            FightingSceneStore.obstacleList,
                            FightingSceneStore.playerList,
                            FightingSceneStore.enemyList,
                            FightingSceneStore.tilemap
                        );
                    }
                    // Cast blackhole
                    StartCoroutine(cast(
                        blackHole,
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

    public void castToNearestPlayer(
        Spell spell,
        Vector3Int target,
        CastSystem CastSystem,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Tilemap tilemap
        )
    {
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
    }
}
