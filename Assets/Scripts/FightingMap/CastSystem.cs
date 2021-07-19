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

            DrawOnMap.showSpellSelection(player.selectedSpellPos, spell.getArea(cellClicked, player, FightingSceneStore.obstacleList, FightingSceneStore.tilemap));

            if (!(player.selectedSpellPos.Count() == spell.clickNb))
            {
                DrawOnMap.showRange(spell.getRange(player, FightingSceneStore.obstacleList, FightingSceneStore.tilemap));
            }

            if (player.selectedSpellPos.Count() == spell.clickNb)
            {
                DrawOnMap.showSpellArea(getTotalArea(spell, player));
                return CastState.CAST_SPELL;
            }
        }

        if (currentState == CastState.CAST_SPELL)
        {
            // If spell area is clear & mana is enough
            if (getTotalArea(spell, player).Contains(cellClicked) && player.currentMana >= spell.manaCost)
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
        FightingSceneStore.EnemiesScrollView.updateScrollView();
        FightingSceneStore.PlayersScrollView.updateScrollView();
        casted = true;
    }

    private List<Vector3Int> getTotalArea(Spell spell, Unit caster)
    {
        List<Vector3Int> area = new List<Vector3Int>();
        // Spell area concatenate each instance of a spell
        caster.selectedSpellPos.ForEach(pos =>
        {
            area = area.Concat(spell.getArea(pos, caster, FightingSceneStore.obstacleList, FightingSceneStore.tilemap)).ToList();
        });
        return area;
    }
}
