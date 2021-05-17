using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    public string name, type;
    public int damage, turnNb, turnCounter;
    public bool permanent, permanentOnTile;
    public Status nextStatus;
    public Status previousStatus;
    public int weight;
    public GameObject tileGO;

    public System.Func<Status, bool> updateFunc { get; set; }
    public System.Func<Status, int> damageFunc { get; set; }
    public System.Func<Status, List<Status>, List<Status>> addStatusToListFunc { get; set; }

    public void setFunctions(
        System.Func<Status, bool> update,
        System.Func<Status, int> damage,
        System.Func<Status, List<Status>, List<Status>> addStatusToList
        )
    {
        updateFunc = update;
        damageFunc = damage;
        addStatusToListFunc = addStatusToList;
    }

    public bool updateStatus()
    {
        return updateFunc(this);
    }

    public int damageStatus()
    {
        return damageFunc(this);
    }

    public List<Status> addStatusToList(List<Status> statusList)
    {
        return addStatusToListFunc(this, statusList);
    }


    public Status(Status status, bool deepCopy = true)
    {
        type = status.type;
        name = status.name;
        damage = status.damage;
        turnNb = status.turnNb;
        permanent = status.permanent;
        permanentOnTile = status.permanentOnTile;
        weight = status.weight;
        tileGO = status.tileGO;
        turnCounter = 0;

        updateFunc = status.updateFunc;
        damageFunc = status.damageFunc;
        addStatusToListFunc = status.addStatusToListFunc;

        // Deep clone
        if (deepCopy)
        {
            Status current = status;
            Status thisCurrent = this;
            // Fill next status
            while (current != null)
            {
                current = current.nextStatus;
                if (current != null)
                {
                    thisCurrent.nextStatus = new Status(current, false);
                    thisCurrent.nextStatus.previousStatus = thisCurrent;
                    thisCurrent = thisCurrent.nextStatus;
                }
            }
            // Fill previous status
            current = status;
            thisCurrent = this;
            while (current != null)
            {
                current = current.previousStatus;
                if (current != null)
                {
                    thisCurrent.previousStatus = new Status(current, false);
                    thisCurrent.previousStatus.nextStatus = thisCurrent;
                    thisCurrent = thisCurrent.previousStatus;
                }
            }
        }
    }

    public Status(string type, string name, int damage = 0, int turnNb = 0, bool permanent = false)
    {
        this.type = type;
        this.name = name;
        this.damage = damage;
        this.turnNb = turnNb;
        this.permanent = permanent;
        this.turnCounter = 0;
        weight = 1;
        nextStatus = null;
        previousStatus = null;
    }

    public Status(string type, int damage = 0, int turnNb = 0, bool permanent = false)
    {
        this.type = type;
        this.name = type;
        this.damage = damage;
        this.turnNb = turnNb;
        this.permanent = permanent;
        this.turnCounter = 0;
        weight = 1;
        nextStatus = null;
        previousStatus = null;
    }

    public override bool Equals(System.Object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Status s = (Status)obj;
            return type == s.type;
        }
    }

    public override int GetHashCode()
    {
        return name.GetHashCode();
    }

    public override string ToString()
    {
        return "Status : " + name + " | Counter : " + turnCounter + " | turnNb : " + turnNb;
    }
}
