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
                player.selectedSpellPos.Clear();
                return CastState.DEFAULT;
            }

            player.selectedSpellPos.Add(cellClicked);
            spell.position = cellClicked;

            DrawOnMap.showSpellSelection(player.selectedSpellPos, spell.getArea(player, FightingSceneStore.obstacleList, FightingSceneStore.tilemap));

            if (!(player.selectedSpellPos.Count() == spell.clickNb))
            {
                DrawOnMap.showRange(spell.getRange(player, FightingSceneStore.obstacleList, FightingSceneStore.tilemap));
            }

            if (player.selectedSpellPos.Count() == spell.clickNb)
            {
                DrawOnMap.showSpellArea(spell, player);
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
            player.selectedSpellPos.Clear();

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
        player.selectedSpellPos.ForEach(sp =>
        {
            spell.instantiateSpell(player, sp, FightingSceneStore.obstacleList, FightingSceneStore.tilemap);
        });
        player.currentMana -= spell.manaCost;
        casted = true;
        /*
        // Animate caster
        spell.animateCaster(player);
        */
    }
}
