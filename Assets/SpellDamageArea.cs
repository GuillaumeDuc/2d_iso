using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellDamageArea : MonoBehaviour
{
    public int turn = 1;
    private int currentTurn;
    private Spell spell;
    void Start()
    {
        currentTurn = turn;
        spell = this.gameObject.GetComponent<Spell>();
        Vector3Int pos = FightingSceneStore.tilemap.WorldToCell(this.gameObject.transform.position);
        FightingSceneStore.spellDamageAreaList.Add(this);
    }

    // True : destroy
    public bool updateTurn()
    {
        currentTurn -= 1;
        return currentTurn == 0;
    }

    public void destroySelf()
    {
        Destroy(this.gameObject);
    }

    public void doDamage()
    {
        this.gameObject.GetComponent<SpellDamage>()?.doDamage(
            spell,
            spell.caster,
            FightingSceneStore.playerList,
            FightingSceneStore.enemyList,
            FightingSceneStore.obstacleList,
            FightingSceneStore.initiativeList,
            FightingSceneStore.tilemap
        );
    }

    public void damageUnit(Unit unit)
    {
        List<Vector3Int> areaSpell = spell.getArea(spell.position, spell.caster, FightingSceneStore.obstacleList, FightingSceneStore.tilemap);
        if (areaSpell.Contains(unit.position))
        {
            bool dead = unit.takeDamage(spell.damage);
            if (dead)
            {
                unit.getTeam().Remove(unit);
                unit.destroySelf();
            }
            FightingSceneStore.TurnBasedSystem.updateScrollViews();
            FightingSceneStore.TurnBasedSystem.tryEndGame();
        }
    }
}
