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
        // Temperature status
        string temperature = "Temperature";

        // No status
        Status NoStatus = new Status(temperature, "No status", 0, 0);
        NoStatus.setFunctions(updateStatus, damageStatus, addStatusToList);
        // Fire status
        Fire = new Status(temperature, "Fire", 10, 3);
        Fire.setFunctions(updateStatus, damageStatus, addStatusToList);
        // Escalating Fire
        Status ExtremeFire = new Status(temperature, "Extreme Fire", 15, 6);
        ExtremeFire.setFunctions(updateStatus, damageStatus, addStatusToList);
        // Escalating More
        Status ScorchingFire = new Status(temperature, "Scorching Fire", 20, 12);
        ScorchingFire.setFunctions(updateStatus, damageStatus, addStatusToList);
        // Freeze Status
        Freeze = new Status(temperature, "Freeze", 10, 3);
        Freeze.setFunctions(updateStatus, damageStatus, addStatusToList);
        // Escalating Freeze
        Status ExtremeFreeze = new Status(temperature, "Extreme Freeze", 15, 6);
        ExtremeFire.setFunctions(updateStatus, damageStatus, addStatusToList);
        // Escalating More
        Status FreezingCold = new Status(temperature, "Freezing Cold", 20, 12);
        FreezingCold.setFunctions(updateStatus, damageStatus, addStatusToList);

        // Setting Temperature status
        List<Status> temperatureList = new List<Status>()
        {
            FreezingCold,
            ExtremeFreeze,
            Freeze,
            NoStatus,
            Fire,
            ExtremeFire,
            ScorchingFire
        };
        setPNStatus(temperatureList);
        setWeight(FreezingCold, -3);

        // Slow Status
        Status Slow = new Status("Slow", "Slowed", 0, 3);

    }

    private void setPNStatus(List<Status> temperatureList)
    {
        Status previous = temperatureList[0];
        temperatureList.ForEach(status =>
        {
            if (previous.name != status.name)
            {
                status.previousStatus = previous;
                previous.nextStatus = status;
                previous = status;
            }
        });
    }

    private void setWeight(Status start, int startWeight)
    {
        Status current = start;
        do
        {
            current.weight = startWeight;
            startWeight += 1;
            current = current.nextStatus;
        } while (current != null);
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

    List<Status> addStatusToList(Status status, List<Status> statusList)
    {
        // Status does not stack
        if (statusList.Contains(status))
        {
            // Get status
            Status currentStatus = statusList.Find(x => x.Equals(status));
            // Reset status
            currentStatus.turnCounter = 0;
            float steps = Mathf.Abs(status.weight);
            // Status weight is inferior to 0, iterate through current status
            if (0 > status.weight)
            {
                int i = 0;
                while (i < steps && currentStatus.previousStatus != null)
                {
                    currentStatus = currentStatus.previousStatus;
                    i++;
                }
            }
            // Status weight is superior to 0, iterate through next status
            if (0 < status.weight)
            {
                int i = 0;
                while (i < steps && currentStatus.nextStatus != null)
                {
                    currentStatus = currentStatus.nextStatus;
                    i++;
                }
            }

            // Current status equals 0, no malus, remove status
            if (0 == currentStatus.weight)
            {
                statusList.Remove(currentStatus);
            }
            // Update status in statusList
            if (0 != currentStatus.weight)
            {
                statusList[statusList.FindIndex(x => x.Equals(status))] = currentStatus;
            }
        }
        // Create new status
        else
        {
            statusList.Add(status);
        }
        return statusList;
    }
}
