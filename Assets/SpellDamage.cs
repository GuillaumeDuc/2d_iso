using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class SpellDamage : MonoBehaviour
{
    public Spell Spell;

    private enum FunctionOption
    {
        DoDamage
    };

    [SerializeField]
    private FunctionOption selectedFunction;
    private Dictionary<FunctionOption, System.Action<Spell, Unit, Dictionary<Unit, GameObject>, Dictionary<Unit, GameObject>, Dictionary<Vector3Int, GameObject>, Tilemap>> functionLookup;

    public void Start()
    {
        functionLookup = new Dictionary<FunctionOption, System.Action<Spell, Unit, Dictionary<Unit, GameObject>, Dictionary<Unit, GameObject>, Dictionary<Vector3Int, GameObject>, Tilemap>>()
        {
            { FunctionOption.DoDamage, SpellDamageList.doDamage }
        };

        doDamage(Spell, Spell.caster, FightingSceneStore.playerList, FightingSceneStore.enemyList, FightingSceneStore.obstacleList, FightingSceneStore.tilemap);
    }

    public void doDamage(Spell spell, Unit caster, Dictionary<Unit, GameObject> playerList, Dictionary<Unit, GameObject> enemyList, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        functionLookup[selectedFunction].Invoke(spell, caster, playerList, enemyList, obstacleList, tilemap);

        // Remove dead
        playerList = playerList
        .Where(p => p.Key.currentHP > 0)
        .ToDictionary(p => p.Key, p => p.Value);

        enemyList = enemyList
        .Where(e => e.Key.currentHP > 0)
        .ToDictionary(e => e.Key, e => e.Value);

        obstacleList = obstacleList
        .Where(o =>
        {
            Obstacle obs = o.Value.GetComponent<Obstacle>();
            return obs.currentHP > 0;
        })
        .ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}
