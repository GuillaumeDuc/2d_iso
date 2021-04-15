using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;

    public int maxHP;
    public int currentHP;
    public List<Spell> spellList = new List<Spell>();
    public Spell selectedSpell;
    public Vector3Int position;

    public bool takeDamage(int dmg)
    {
        currentHP -= dmg;

        return currentHP <= 0;
    }

    public void setSpellList(List<Spell> newSpells)
    {
        foreach (var s in newSpells)
        {
            spellList.Add(s);
        }
    }

    public void setSpellList(Spell newSpells)
    {
        spellList.Add(newSpells);
    }

    public void setStats(string name, int maxHP)
    {
        this.unitName = name;
        this.maxHP = maxHP;
        this.currentHP = maxHP;
    }

    public void setStats(string name, int maxHP, Vector3Int position)
    {
        setStats(name, maxHP);
        this.position = position;
    }

    public string getSpellList()
    {
        string spellList = "";
        foreach (var s in spellList)
        {
            spellList += s.ToString() + "\n";
        }
        return spellList;
    }

    public override bool Equals(System.Object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Unit u = (Unit)obj;
            return unitName == u.unitName && position == u.position;
        }
    }

    public override int GetHashCode()
    {
        return unitName.GetHashCode();
    }

    public override string ToString()
    {
        return "Unit " + unitName + "\n" +
            "max HP : " + maxHP + " - current HP : " + currentHP + "\n" +
            "position : " + position;
    }
}
