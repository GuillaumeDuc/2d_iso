using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class SpellDamage : MonoBehaviour
{
    private Spell spell;
    public float damageDelay = 0;

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
        StartCoroutine(delayDamage(
            spell,
            spell.caster,
            FightingSceneStore.playerList,
            FightingSceneStore.enemyList,
            FightingSceneStore.obstacleList,
            FightingSceneStore.initiativeList,
            FightingSceneStore.tilemap
            )
        );
    }

    private IEnumerator delayDamage(
        Spell spell,
        Unit caster,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Dictionary<Unit, bool> initiativeList,
        Tilemap tilemap
        )
    {
        yield return new WaitForSeconds(damageDelay);
        doDamage(
            spell,
            caster,
            playerList,
            enemyList,
            obstacleList,
            initiativeList,
            tilemap
        );
    }

    public void doDamage(
        Spell spell,
        Unit caster,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Dictionary<Unit, bool> initiativeList,
        Tilemap tilemap
        )
    {
        functionLookup[selectedFunction].Invoke(spell, caster, playerList, enemyList, obstacleList, tilemap);
        removeDead(playerList, enemyList, obstacleList, initiativeList);
        updateScrollViews();
        FightingSceneStore.CastSystem.casted = true;
    }

    private void removeDead(
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Dictionary<Unit, bool> initiativeList
        )
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
            playerList.Remove(s);
            initiativeList.Remove(s);
            s.destroySelf();
        }
        foreach (var s in deadEnemyList)
        {
            enemyList.Remove(s);
            initiativeList.Remove(s);
            s.destroySelf();
        }
        foreach (var s in destroyedObstacleList)
        {
            try
            {
                obstacleList[s].GetComponent<Obstacle>().destroySelf();
                obstacleList.Remove(s);
            }
            catch { }
        }
    }

    public void updateScrollViews()
    {
        FightingSceneStore.PlayersScrollView.updateScrollView();
        FightingSceneStore.EnemiesScrollView.updateScrollView();
    }
}
