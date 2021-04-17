using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffect
{
    Status[] statusList;
    public int value;

    public System.Action<Spell, SpellEffect, Dictionary<Unit, GameObject>, Dictionary<Unit, GameObject>, Dictionary<Vector3Int, GameObject>, Tilemap> applyEffectAction;

    public void applyEffect(
        Spell spell,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        applyEffectAction(spell, this, playerList, enemyList, obstacleList, tilemap);
    }

    public SpellEffect()
    {
        value = 0;
    }

    public SpellEffect(int value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        return "Effect : " + value;
    }
}
