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

    public bool casted = false, isCasting = false, isDamaging = false, isEffect = false;

    private Tile threeSidesTile, threeSidesBottomTile, twoSidesLeftTile, twoSidesRightTile, transparent;

    public CastState cast(
        Spell spell,
        Unit player,
        Vector3Int cellClicked,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        CastState currentState,
        Tilemap tilemap,
        Tilemap cellsGrid
        )
    {
        if (currentState == CastState.SHOW_AREA)
        {
            if (!spell.canCast(player, cellClicked, obstacleList, tilemap))
            {
                spell.spellPos.Clear();
                return CastState.DEFAULT;
            }

            spell.spellPos.Add(cellClicked);

            DrawOnMap.showSpellSelection(spell.spellPos, spell.getArea(obstacleList, tilemap));

            if (!(spell.spellPos.Count() == spell.clickNb))
            {
                DrawOnMap.showRange(spell.getRange(player, obstacleList, tilemap));
            }

            if (spell.spellPos.Count() == spell.clickNb)
            {
                DrawOnMap.showSpellArea(spell.getArea(obstacleList, tilemap));
                return CastState.CAST_SPELL;
            }
        }

        if (currentState == CastState.CAST_SPELL)
        {
            // If spell area is clear & mana is enough
            if (spell.getArea(obstacleList, tilemap).Contains(cellClicked) && player.currentMana >= spell.manaCost)
            {
                castSpell(spell, player, playerList, enemyList, obstacleList, tilemap);
            }
            spell.spellPos.Clear();

            return CastState.DEFAULT;
        }

        return currentState;
    }

    public void castSpell(
        Spell s,
        Unit player,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        // Save previous spell caracteristics
        Spell spell = new Spell(s);

        isCasting = true;
        isDamaging = true;
        isEffect = true;

        // Damage
        StartCoroutine(delayDamage(spell, playerList, enemyList, obstacleList, tilemap));

        // Effect
        if (spell.delayEffect != 0)
        {
            StartCoroutine(delayEffect(spell, player, playerList, enemyList, obstacleList, tilemap));
        }
        else
        {
            spell.applyEffect(player, playerList, enemyList, obstacleList, tilemap);
            isEffect = false;
        }

        // Animation
        spell.playAnimation(obstacleList, tilemap);

        // Animate caster
        spell.animateCaster(player);

        // Mana
        player.currentMana -= spell.manaCost;
        updateScrollViews(playerList, enemyList);

        updateIsCasting();
    }

    public IEnumerator delayEffect(
        Spell spell,
        Unit player,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        yield return new WaitForSeconds(spell.delayEffect);
        spell.applyEffect(player, playerList, enemyList, obstacleList, tilemap);
        isEffect = false;
        updateIsCasting();
    }

    public IEnumerator delayDamage(
        Spell spell,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        yield return new WaitForSeconds(spell.delayDamage);
        spell.doDamage(playerList, enemyList, obstacleList, tilemap);
        updateScrollViews(playerList, enemyList);
        isDamaging = false;
        updateIsCasting();
    }

    public void updateScrollViews(
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList
        )
    {
        EnemiesScrollView.updateScrollView();
        PlayersScrollView.updateScrollView();
    }

    private void updateIsCasting()
    {
        if (!isDamaging && !isEffect)
        {
            isCasting = false;
            casted = true;
        }
    }
}
