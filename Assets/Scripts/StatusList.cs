using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class StatusList : MonoBehaviour
{
    public Status
        Fire
        ;

    void Start()
    {
        Fire = new Status("Fire", 10, 3);
        Fire.updateFunc = updateStatus;
        Fire.damageFunc = damageStatus;
    }

    bool updateStatus(Status status)
    {
        status.turnCounter += 1;
        return status.turnCounter < status.turnNb || status.permanent;
    }

    int damageStatus(Status status)
    {
        return status.damage;
    }
}
