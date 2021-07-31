using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel,
        maxHP = 100,
        endurance = 0,
        mana = 100,
        spellSlot = 3,
        movementPoint = 6;

    [HideInInspector]
    public int initiative,
            currentHP,
            currentMovementPoint,
            currentMana;
    public List<GameObject> spellList = new List<GameObject>();
    [HideInInspector]
    public GameObject selectedSpell;
    [HideInInspector]
    public Vector3Int position;
    public bool playable = false;
    [HideInInspector]
    public bool summon = false, isPlaying = false;
    [HideInInspector]
    public List<Status> statusList = new List<Status>();
    [HideInInspector]
    public List<Vector3Int> selectedSpellPos = new List<Vector3Int>();

    public Dictionary<Unit, GameObject> getTeam()
    {
        return FightingSceneStore.playerList.ContainsKey(this) ? FightingSceneStore.playerList : FightingSceneStore.enemyList;
    }

    public Dictionary<Unit, GameObject> getEnemyTeam()
    {
        return FightingSceneStore.enemyList.ContainsKey(this) ? FightingSceneStore.playerList : FightingSceneStore.enemyList;
    }

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
            return name == u.name && position == u.position;
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
        return "Unit : " + name + " | name : " + unitName + " | HP : " + currentHP + "\n" +
            "Movement : " + currentMovementPoint + " | Mana : " + currentMana + "\n" +
            "list status : \n" + list;
    }

    void Start()
    {
        this.maxHP = (int)(maxHP * (1 + (double)endurance / 10));
        this.currentHP = this.maxHP;

        this.position = FightingSceneStore.tilemap.WorldToCell(gameObject.transform.position);

        this.movementPoint = movementPoint + (int)(endurance / 10);
        this.currentMovementPoint = movementPoint;

        this.currentMana = mana;

        initiative = mana + endurance;

        FightingSceneStore.TurnBasedSystem.addUnitInInitList(this);
    }
}
