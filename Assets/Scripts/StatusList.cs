using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class StatusList : MonoBehaviour
{
    public Status
        Fire,
        Freeze
        ;

    void Start()
    {
        // Fire status
        Fire = new Status("Fire", 10, 3);
        Fire.updateFunc = updateStatus;
        Fire.damageFunc = damageStatus;

        // Freeze Status
        Freeze = new Status("Freeze", 10, 3);
        Freeze.updateFunc = updateStatus;
        Freeze.damageFunc = damageStatus;
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