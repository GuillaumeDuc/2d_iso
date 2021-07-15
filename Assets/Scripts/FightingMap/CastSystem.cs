using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.Linq;
using System.Collections;

public class CastSystem : MonoBehaviour
{
    public InfoScrollView EnemiesScrollView;

    public InfoScrollView PlayersScrollView;

    public DrawOnMap DrawOnMap;

    public bool casted = false;

    public CastState cast(
        GameObject spellGO,
        Unit player,
        Vector3Int cellClicked,
        CastState currentState
        )
    {
        Spell spell = spellGO.GetComponent<Spell>();
        if (currentState == CastState.SHOW_AREA)
        {
            if (!spell.getRange(player, FightingSceneStore.obstacleList, FightingSceneStore.tilemap).Contains(cellClicked))
            {
                spell.spellPos.Clear();
                return CastState.DEFAULT;
            }

            spell.spellPos.Add(cellClicked);

            DrawOnMap.showSpellSelection(spell.spellPos, spell.getArea(player, FightingSceneStore.obstacleList, FightingSceneStore.tilemap));

            if (!(spell.spellPos.Count() == spell.clickNb))
            {
                DrawOnMap.showRange(spell.getRange(player, FightingSceneStore.obstacleList, FightingSceneStore.tilemap));
            }

            if (spell.spellPos.Count() == spell.clickNb)
            {
                DrawOnMap.showSpellArea(spell.getArea(player, FightingSceneStore.obstacleList, FightingSceneStore.tilemap));
                return CastState.CAST_SPELL;
            }
        }

        if (currentState == CastState.CAST_SPELL)
        {
            // If spell area is clear & mana is enough
            if (spell.getArea(player, FightingSceneStore.obstacleList, FightingSceneStore.tilemap).Contains(cellClicked) && player.currentMana >= spell.manaCost)
            {
                castSpell(spell, player);
            }
            spell.spellPos.Clear();

            return CastState.DEFAULT;
        }

        return currentState;
    }

    public void castSpell(
        Spell spell,
        Unit player
        )
    {
        spell.caster = player;
        spell.spellPos.ForEach(pos =>
        {
            spell.instantiateSpell(player, FightingSceneStore.obstacleList, FightingSceneStore.tilemap);
        });
        player.currentMana -= spell.manaCost;
        casted = true;
        /*
        // Animate caster
        spell.animateCaster(player);
        */
    }
}
