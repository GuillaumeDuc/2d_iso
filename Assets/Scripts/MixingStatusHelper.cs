using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MixingStatusHelper
{
    public List<Status> getStatusList(StatusList StatusList, List<Status> statusList, Status status)
    {
        // Check is statusList is empty
        if (!statusList.Any())
        {
            statusList.Add(status);
            return statusList;
        }

        List<Status> newList = new List<Status>();
        bool mixed = false;
        Status currentStatus = status;

        // For each status in list try to mix status with status in list
        statusList.ForEach(s =>
        {
            // Mixing with Temperature
            if (currentStatus.type == StatusList.Fire.type)
            {
                Status add = mixTemperature(StatusList, currentStatus, s);
                // If spells got mixed
                if (add != null)
                {
                    addMixedStatus(add, s, newList);
                    currentStatus = add;
                    mixed = true;
                }
                else
                // Not mixing with Temperature
                {
                    addStatusInList(s, newList);
                }
            }
            // Mixing with Wet
            else if (currentStatus.type == StatusList.Wet.type)
            {
                Status add = mixWet(StatusList, currentStatus, s);
                // If spells got mixed
                if (add != null)
                {
                    addMixedStatus(add, s, newList);
                    currentStatus = add;
                    mixed = true;
                }
                else
                // Not mixing with Wet
                {
                    addStatusInList(s, newList);
                }
            }
            // Not mixing with anything
            else
            {
                addStatusInList(s, newList);
            }
        });

        if (!mixed)
        {
            addStatusInList(status, newList);
        }
        return newList;
    }

    private Status mixTemperature(StatusList StatusList, Status temperature, Status status)
    {
        // Steam = Wet + Fire
        if (temperature.weight > 0 && status.type == StatusList.Wet.type)
        {
            return new Status(StatusList.Steam);
        }
        return null;
    }

    private Status mixWet(StatusList StatusList, Status Wet, Status status)
    {
        // Steam = Wet + Fire
        if (status.type == StatusList.Fire.type && status.weight > 0)
        {
            return new Status(StatusList.Steam);
        }

        return null;
    }

    private void addStatusInList(Status status, List<Status> list)
    {
        if (!list.Contains(status))
        {
            list.Add(status);
        }
        else
        {
            // Update status in statusList
            list[list.FindIndex(x => x.Equals(status))] = status;
        }
    }

    private void addMixedStatus(Status add, Status s, List<Status> newList)
    {
        // If spell is permanent on tile, put it again
        if (s.permanentOnTile)
        {
            newList.Add(s);
        }
        // Add mixed to list
        addStatusInList(add, newList);
    }
}
