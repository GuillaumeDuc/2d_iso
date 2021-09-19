using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public static class StatusList
{
    private static MixingStatusHelper MixingStatusHelper = new MixingStatusHelper();

    ////// Temperature //////
    // Temperature Tile GameObject
    static GameObject fireTileEffect = Resources.Load<GameObject>("TileEffects/Fire/FireEffect");
    static GameObject extremeFireTileEffect = Resources.Load<GameObject>("TileEffects/Fire/ExtremeFireEffect");
    static GameObject scorchingFireTileEffect = Resources.Load<GameObject>("TileEffects/Fire/ScorchingFireEffect");
    static GameObject freezeTileEffect = Resources.Load<GameObject>("TileEffects/Freeze/FreezeEffect");
    static GameObject extremeFreezeTileEffect = Resources.Load<GameObject>("TileEffects/Freeze/ExtremFreezeEffect");
    static GameObject freezingColdTileEffect = Resources.Load<GameObject>("TileEffects/Freeze/FreezingColdEffect");

    // Temperature status
    static string temperature = "Temperature";

    // No status
    static Status NoStatus = new Status(temperature, "No status", 0, 0);
    // Fire status
    public static Status Fire = new Status(temperature, "Fire", 10, 3, false, fireTileEffect);
    // Escalating Fire
    static Status ExtremeFire = new Status(temperature, "Extreme Fire", 15, 6, false, extremeFireTileEffect);
    // Escalating More
    static Status ScorchingFire = new Status(temperature, "Scorching Fire", 20, 12, false, scorchingFireTileEffect);
    // Freeze Status
    public static Status Freeze = new Status(temperature, "Freeze", 10, 3, false, freezeTileEffect, walkableWater);
    // Escalating Freeze
    static Status ExtremeFreeze = new Status(temperature, "Extreme Freeze", 15, 6, false, freezeTileEffect, walkableWater);
    // Escalating More
    static Status FreezingCold = new Status(temperature, "Freezing Cold", 20, 12, false, freezeTileEffect, walkableWater);
    // Setting Temperature status
    static List<Status> temperatureList = setWeight(setPNStatus(setFunctionsStatus(new List<Status>()
        {
            FreezingCold,
            ExtremeFreeze,
            Freeze,
            NoStatus,
            Fire,
            ExtremeFire,
            ScorchingFire
        })), -3);

    ////// Slow Status //////
    public static Status Slow = setFunctions(new Status("Slow", 0, 3));

    ////// Wet Status //////
    public static Status Wet = setFunctions(new Status("Wet", 0, 3, false, null, null, true));

    ////// Steam Status //////
    public static GameObject steamEffect = Resources.Load<GameObject>("TileEffects/Steam/SteamEffect");
    public static Status Steam = setFunctions(new Status("Steam", 0, 3, false, steamEffect, removeLineOfSight));

    ////// Entrap Status //////
    public static Status Entrap = setFunctions(new Status("Entrap", 0, 3));


    public static List<Status> getStatuses()
    {
        return new List<Status>()
        {
            Fire,
            Slow,
            Wet,
            Steam,
            Entrap
        };
    }

    private static Status setFunctions(Status status)
    {
        status.setFunctions(
            updateStatus,
            damageStatus,
            addStatusToPlayer,
            addStatusToTile
        );
        return status;
    }

    private static List<Status> setFunctionsStatus(List<Status> list)
    {
        list.ForEach(status =>
        {
            status.setFunctions(updateStatus, damageStatus, addStatusToPlayer, addStatusToTile);
        });
        return list;
    }

    private static List<Status> setPNStatus(List<Status> temperatureList)
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
        return temperatureList;
    }

    private static List<Status> setWeight(List<Status> list, int startWeight)
    {
        Status current = list[0];
        do
        {
            current.weight = startWeight;
            startWeight += 1;
            current = current.nextStatus;
        } while (current != null);

        return list;
    }

    static bool updateStatus(Status status)
    {
        status.turnCounter += 1;
        return status.turnCounter < status.turnNb || status.permanent;
    }

    static int damageStatus(Status status)
    {
        return status.damage;
    }

    static List<Status> addStatusToPlayer(Status status, List<Status> statusList)
    {
        Status newStatus = new Status(status);
        // Remove permanent on tile property for players
        newStatus.permanentOnTile = false;
        return addStatusToList(newStatus, statusList);
    }

    static List<Status> addStatusToTile(Status status, List<Status> statusList)
    {
        return addStatusToList(status, statusList);
    }

    static List<Status> addStatusToList(Status status, List<Status> statusList)
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
            statusList = MixingStatusHelper.getStatusList(statusList, status);
        }
        return statusList;
    }

    // Modify Tile 

    static public void removeLineOfSight(Status status, Tile tile)
    {
        GroundTile gt = (GroundTile)tile;
        gt.lineOfSight = false;
    }

    static public void walkableWater(Status status, Tile tile)
    {
        WaterTile wt;
        if (tile is WaterTile)
        {
            wt = (WaterTile)tile;
            wt.walkable = true;
        }
    }
}
