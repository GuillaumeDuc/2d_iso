using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;

    public int maxHP;
    public int currentHP;
    public List<Spell> spells = new List<Spell>();

    public bool takeDamage(int dmg)
    {
        currentHP -= dmg;

        return currentHP <= 0;
    }

    public void setSpellList(List<Spell> newSpells)
    {
        foreach (var s in newSpells)
        {
            spells.Add(s);
        }
    }

    public void setSpellList(Spell newSpells)
    {
        spells.Add(newSpells);
    }

    public void setStats(string name, int maxHP, int currentHP)
    {
        this.unitName = name;
        this.maxHP = maxHP;
        this.currentHP = currentHP;
    }

    public string getSpellList()
    {
        string spellList = "";
        foreach (var s in spells)
        {
            spellList += s.ToString() + "\n";
        }
        return spellList;
    }
}
