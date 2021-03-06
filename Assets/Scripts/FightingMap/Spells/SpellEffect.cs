using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellEffect
{
    public string name;
    public List<Status> statusList;
    public List<Status> statusTileList;
    public int value;
    public bool cumul;

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

    public SpellEffect(string name, int value = 0, bool cumul = true)
    {
        this.name = name;
        value = 0;
        this.cumul = cumul;
        statusList = new List<Status>();
        statusTileList = new List<Status>();
    }

    public override string ToString()
    {
        return "Effect : " + name;
    }

    public override bool Equals(System.Object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            SpellEffect s = (SpellEffect)obj;
            return name == s.name;
        }
    }

    public override int GetHashCode()
    {
        return name.GetHashCode();
    }
}
