using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Linq;

public class SpellEffectScript : MonoBehaviour
{
    private Spell spell;
    public float effectDelay = 0;
    [HideInInspector]
    public List<SpellEffect> selectedSpellEffects = new List<SpellEffect>();

    private void Start()
    {
        // Set spell
        Spell spellGo = gameObject.GetComponent<Spell>();
        if (spellGo != null) { spell = spellGo; }

        // Search for originals SpellEfects
        selectedSpellEffects = selectedSpellEffects.Select(a =>
        {
            return SpellEffectList.spellEffects.Find(s => s.Equals(a));
        }).ToList();
        // Save obstacles before impact
        Dictionary<Vector3Int, GameObject> obstacleList = FightingSceneStore.obstacleList.ToDictionary(entry => entry.Key, entry => entry.Value);
        StartCoroutine(delayEffect(
            FightingSceneStore.playerList,
            FightingSceneStore.enemyList,
            obstacleList,
            FightingSceneStore.tilemap
        ));
    }

    private IEnumerator delayEffect(
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        yield return new WaitForSeconds(effectDelay);
        applyEffect(
            playerList,
            enemyList,
            obstacleList,
            tilemap
        );
        FightingSceneStore.TurnBasedSystem.DrawOnMap.resetMap();
    }

    public void applyEffect(
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        selectedSpellEffects.ForEach(se =>
        {
            se.applyEffect(spell, playerList, enemyList, obstacleList, tilemap);
        });
    }
}