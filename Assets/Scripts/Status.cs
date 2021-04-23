using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    public string name;
    public int damage, turnNb, turnCounter;
    public bool permanent;

    public System.Func<Status, bool> updateFunc { get; set; }
    public System.Func<Status, int> damageFunc { get; set; }

    public bool updateStatus()
    {
        return updateFunc(this);
    }

    public int damageStatus()
    {
        return damageFunc(this);
    }

    public Status (Status status)
    {
        name = status.name;
        damage = status.damage;
        turnNb = status.turnNb;
        permanent = status.permanent;
        updateFunc = status.updateFunc;
        damageFunc = status.damageFunc;
        turnCounter = 0;
    }

    public Status(string name, int damage = 0, int turnNb = 0, bool permanent = false)
    {
        this.name = name;
        this.damage = damage;
        this.turnNb = turnNb;
        this.permanent = permanent;
        this.turnCounter = 0;
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
            return name == s.name;
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
