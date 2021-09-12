using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int maxHP;
    [HideInInspector]
    public int currentHP;
    public bool hideLineOfSight = true, preventsWalk = true;
    public List<Status> statusList = new List<Status>();

    void Start()
    {
        currentHP = maxHP;
    }

    public bool takeDamage(int dmg)
    {
        currentHP -= dmg;

        return currentHP <= 0;
    }

    public void addStatus(Status status)
    {
        statusList.Add(status);
    }

    public virtual void updateStatus()
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
}
