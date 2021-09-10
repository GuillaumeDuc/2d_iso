using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class StatusList : MonoBehaviour
{
    public Status Fire,
        Freeze,
        Slow,
        Wet,
        Steam
        ;

    private MixingStatusHelper MixingStatusHelper = new MixingStatusHelper();

    void Start()
    {
        ////// Temperature //////
        // Temperature Tile GameObject
        GameObject fireTileEffect = Resources.Load<GameObject>("TileEffects/Fire/FireEffect");
        GameObject extremeFireTileEffect = Resources.Load<GameObject>("TileEffects/Fire/ExtremeFireEffect");
        GameObject scorchingFireTileEffect = Resources.Load<GameObject>("TileEffects/Fire/ScorchingFireEffect");
        GameObject freezeTileEffect = Resources.Load<GameObject>("TileEffects/Freeze/FreezeEffect");
        GameObject extremeFreezeTileEffect = Resources.Load<GameObject>("TileEffects/Freeze/ExtremFreezeEffect");
        GameObject freezingColdTileEffect = Resources.Load<GameObject>("TileEffects/Freeze/FreezingColdEffect");

        // Temperature status
        string temperature = "Temperature";

        // No status
        Status NoStatus = new Status(temperature, "No status", 0, 0);
        // Fire status
        Fire = new Status(temperature, "Fire", 10, 3);
        Fire.tileGO = fireTileEffect;
        // Escalating Fire
        Status ExtremeFire = new Status(temperature, "Extreme Fire", 15, 6);
        ExtremeFire.tileGO = extremeFireTileEffect;
        // Escalating More
        Status ScorchingFire = new Status(temperature, "Scorching Fire", 20, 12);
        ScorchingFire.tileGO = scorchingFireTileEffect;
        // Freeze Status
        Freeze = new Status(temperature, "Freeze", 10, 3);
        Freeze.tileGO = freezeTileEffect;
        Freeze.modifyTileAction = walkableWater;
        // Escalating Freeze
        Status ExtremeFreeze = new Status(temperature, "Extreme Freeze", 15, 6);
        ExtremeFreeze.tileGO = freezeTileEffect;
        ExtremeFreeze.modifyTileAction = walkableWater;
        // Escalating More
        Status FreezingCold = new Status(temperature, "Freezing Cold", 20, 12);
        FreezingCold.tileGO = freezeTileEffect;
        FreezingCold.modifyTileAction = walkableWater;
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
        setFunctionsStatus(temperatureList);
        setPNStatus(temperatureList);
        setWeight(FreezingCold, -3);

        ////// Slow Status //////
        Slow = new Status("Slow", 0, 3);
        setFunctions(Slow);

        ////// Wet Status //////
        Wet = new Status("Wet", 0, 3);
        Wet.permanentOnTile = true;
        setFunctions(Wet);

        ////// Steam Status //////
        Steam = new Status("Steam", 0, 3);
        setFunctions(Steam);
        Steam.modifyTileAction = removeLineOfSight;
        GameObject steamEffect = Resources.Load<GameObject>("TileEffects/Steam/SteamEffect");
        Steam.tileGO = steamEffect;
    }

    private void setFunctions(Status status)
    {
        status.setFunctions(
            updateStatus,
            damageStatus,
            addStatusToPlayer,
            addStatusToTile
            );
    }

    private void setFunctionsStatus(List<Status> list)
    {
        list.ForEach(status =>
        {
            status.setFunctions(updateStatus, damageStatus, addStatusToPlayer, addStatusToTile);
        });
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

    List<Status> addStatusToPlayer(Status status, List<Status> statusList)
    {
        Status newStatus = new Status(status);
        // Remove permanent on tile property for players
        newStatus.permanentOnTile = false;
        return addStatusToList(newStatus, statusList);
    }

    List<Status> addStatusToTile(Status status, List<Status> statusList)
    {
        return addStatusToList(status, statusList);
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
            statusList = MixingStatusHelper.getStatusList(this, statusList, status);
        }
        return statusList;
    }

    // Modify Tile 

    public void removeLineOfSight(Status status, Tile tile)
    {
        GroundTile gt = (GroundTile)tile;
        gt.lineOfSight = false;
    }

    public void walkableWater(Status status, Tile tile)
    {
        WaterTile wt;
        if (tile is WaterTile)
        {
            wt = (WaterTile)tile;
            wt.walkable = true;
        }
    }
}
