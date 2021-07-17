﻿using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel,
        maxHP,
        endurance,
        mana,
        spellSlot,
        initiative,
        movementPoint,
        currentHP,
        currentMovementPoint,
        currentMana
        ;
    public List<GameObject> spellList = new List<GameObject>();
    public GameObject selectedSpell;
    public Vector3Int position;
    public bool playable;
    public List<Status> statusList = new List<Status>();
    public GameObject unitGO;
    public List<Vector3Int> selectedSpellPos = new List<Vector3Int>();

    public bool takeDamage(int dmg)
    {
        currentHP -= dmg;

        return currentHP <= 0;
    }

    public void setSpellList(List<GameObject> newSpells)
    {
        foreach (var s in newSpells)
        {
            setSpellList(s);
        }
    }

    public void setSpellList(GameObject newSpells)
    {
        spellList.Add(newSpells);
    }

    public void resetStats()
    {
        currentMovementPoint = movementPoint;
        currentMana = mana;
    }

    public void setStats(
        GameObject unitGO,
        string name,
        Vector3Int position,
        int maxHP = 100,
        int endurance = 0,
        int mana = 100,
        int movementPoint = 6,
        int spellSlot = 3
        )
    {
        this.unitGO = unitGO;

        this.unitName = name;
        this.maxHP = (int)(maxHP * (1 + (double)endurance / 10));
        this.currentHP = this.maxHP;

        this.position = position;

        this.endurance = endurance;
        this.mana = mana;
        this.movementPoint = movementPoint + (int)(endurance / 10);
        this.currentMana = mana;
        this.currentMovementPoint = movementPoint;

        initiative = mana + endurance;

        this.playable = false;
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

    public void addStatus(Status status)
    {
        Status newStatus = new Status(status);
        statusList = newStatus.addStatusToPlayer(statusList);
    }

    public void updateStatus()
    {
        List<Status> newStatusList = new List<Status>();
        statusList.ForEach(s =>
        {
            bool continueStatus = s.updateStatus();
            if (continueStatus)
            {
                newStatusList.Add(s);
            }
        });
        statusList = newStatusList;
    }

    public void takeStatus()
    {
        statusList.ForEach(s =>
        {
            takeDamage(s.damageStatus());
        });
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
        string list = "";
        statusList.ForEach(s =>
        {
            list += s + "\n";
        });
        return "Unit : " + unitName + " | HP : " + currentHP + "\n" +
            "Movement : " + currentMovementPoint + " | Mana : " + currentMana + "\n" +
            "list status : \n" + list;
    }
}
