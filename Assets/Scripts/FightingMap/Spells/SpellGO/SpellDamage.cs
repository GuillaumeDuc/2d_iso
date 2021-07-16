using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class SpellDamage : MonoBehaviour
{
    private Spell spell;

    private enum FunctionOption
    {
        DoDamage
    };

    [SerializeField]
    private FunctionOption selectedFunction;
    private Dictionary<FunctionOption, System.Action<Spell, Unit, Dictionary<Unit, GameObject>, Dictionary<Unit, GameObject>, Dictionary<Vector3Int, GameObject>, Tilemap>> functionLookup = new Dictionary<FunctionOption, System.Action<Spell, Unit, Dictionary<Unit, GameObject>, Dictionary<Unit, GameObject>, Dictionary<Vector3Int, GameObject>, Tilemap>>()
        {
            { FunctionOption.DoDamage, SpellDamageList.doDamage }
        };

    public void Start()
    {
        // Set spell
        Spell spellGo = gameObject.GetComponent<Spell>();
        if (spellGo != null) { spell = spellGo; }
        doDamage(spell, spell.caster, FightingSceneStore.playerList, FightingSceneStore.enemyList, FightingSceneStore.obstacleList, FightingSceneStore.tilemap);
    }

    public void doDamage(Spell spell, Unit caster, Dictionary<Unit, GameObject> playerList, Dictionary<Unit, GameObject> enemyList, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        functionLookup[selectedFunction].Invoke(spell, caster, playerList, enemyList, obstacleList, tilemap);
        removeDead(playerList, enemyList, obstacleList);
        updateScrollViews();
    }

    private void removeDead(Dictionary<Unit, GameObject> playerList, Dictionary<Unit, GameObject> enemyList, Dictionary<Vector3Int, GameObject> obstacleList)
    {

        List<Unit> deadPlayerList = new List<Unit>();
        List<Unit> deadEnemyList = new List<Unit>();
        List<Vector3Int> destroyedObstacleList = new List<Vector3Int>();

        // Remove dead from list
        deadPlayerList = playerList
        .Where(p => p.Key.currentHP <= 0)
        .Select(p => p.Key)
        .ToList();

        deadEnemyList = enemyList
        .Where(e => e.Key.currentHP <= 0)
        .Select(p => p.Key)
        .ToList();

        destroyedObstacleList = obstacleList
        .Where(o =>
        {
            Obstacle obs = o.Value.GetComponent<Obstacle>();
            return obs.currentHP <= 0;
        })
        .Select(p => p.Key)
        .ToList();

        // Remove dead
        foreach (var s in deadPlayerList)
        {
            Destroy(playerList[s]);
            playerList.Remove(s);
        }
        foreach (var s in deadEnemyList)
        {
            Destroy(enemyList[s]);
            enemyList.Remove(s);
        }
        foreach (var s in destroyedObstacleList)
        {
            Destroy(obstacleList[s]);
            obstacleList.Remove(s);
        }
    }

    public void updateScrollViews()
    {
        FightingSceneStore.PlayersScrollView.updateScrollView();
        FightingSceneStore.EnemiesScrollView.updateScrollView();
    }
}
