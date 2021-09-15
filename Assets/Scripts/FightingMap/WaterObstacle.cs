using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaterObstacle : Obstacle
{
    private int waterStep = 3;

    void Start()
    {
        // Add status to tile & player
        Vector3Int pos = FightingSceneStore.tilemap.WorldToCell(this.gameObject.transform.position);
        GroundTile tile = (GroundTile)FightingSceneStore.tilemap.GetTile(pos);
        if (tile != null && !(tile is WaterTile))
        {
            tile.addStatus(new Status(StatusList.Wet));
            FightingSceneStore.tilemap.RefreshTile(pos);
        }
        Dictionary<Unit, GameObject> allCharacters = FightingSceneStore.playerList.Concat(FightingSceneStore.enemyList).ToDictionary(x => x.Key, x => x.Value);
        Unit u = getUnitFromPos(allCharacters, pos);
        if (u != null)
        {
            u.addStatus(new Status(StatusList.Wet));
        }
    }

    private Unit getUnitFromPos(Dictionary<Unit, GameObject> dict, Vector3Int cell)
    {
        foreach (var d in dict)
        {
            if (d.Key.position == cell)
            {
                return d.Key;
            }
        }
        return null;
    }

    public override void updateStatus()
    {
        base.updateStatus();
        if (waterStep > 1)
        {
            bool splitWater = split();
            if (splitWater)
            {
                changeGO();
            }
        }
        else
        {
            try
            {
                FightingSceneStore.obstacleList.Remove(FightingSceneStore.tilemap.WorldToCell(gameObject.transform.position));
            }
            catch { }
            DestroyImmediate(this.gameObject);
        }
    }

    void changeGO()
    {
        if (waterStep == 2)
        {
            Sprite water = Resources.Load<Sprite>("Obstacles/Water/water_column2_sprite");
            this.gameObject.GetComponent<SpriteRenderer>().sprite = water;
            this.hideLineOfSight = false;
        }
        else if (waterStep == 1)
        {
            Sprite water = Resources.Load<Sprite>("Obstacles/Water/water_column3_sprite");
            this.gameObject.GetComponent<SpriteRenderer>().sprite = water;
        }
    }

    void setNewWater(GameObject go)
    {
        waterStep = go.GetComponent<WaterObstacle>().getWaterStep();
        changeGO();
    }

    public int getWaterStep()
    {
        return waterStep;
    }

    public void setWaterStep(int step)
    {
        waterStep = step;
    }

    bool split()
    {
        Vector3Int cellPos = FightingSceneStore.tilemap.WorldToCell(gameObject.transform.position);
        List<Vector3Int> listO = new List<Vector3Int>(RangeUtils.getNeighours(cellPos)) { cellPos };
        // If all neighbours are water tile and same height, no split
        bool isSplitting = listO.Exists(cell =>
        {
            // Get obstacle
            Obstacle existingObstacle = null;
            if (FightingSceneStore.obstacleList.ContainsKey(cell))
            {
                existingObstacle = FightingSceneStore.obstacleList[cell].GetComponent<Obstacle>();
            }

            // Check if neighbour is not an obstacle or water obstacle & different step & waterstep is > 1
            return existingObstacle == null || ((existingObstacle is WaterObstacle) && (waterStep > (((WaterObstacle)existingObstacle).getWaterStep())) && waterStep > 1);
        });
        if (!isSplitting)
        {
            // No splitting
            return false;
        }

        // Split
        waterStep -= 1;
        listO.ForEach(pos =>
        {
            // Get obstacle
            Obstacle existingObstacle = null;
            if (FightingSceneStore.obstacleList.ContainsKey(pos))
            {
                existingObstacle = FightingSceneStore.obstacleList[pos].GetComponent<Obstacle>();
            }

            // Check if neighbour is not already a water obstacle & not an empty tile
            if (!(existingObstacle is WaterObstacle) && FightingSceneStore.tilemap.GetTile(pos) != null)
            {
                Vector2 worldPos = FightingSceneStore.tilemap.CellToWorld(pos);

                GameObject obstacle = Instantiate(this.gameObject, worldPos, Quaternion.identity);
                obstacle.GetComponent<WaterObstacle>().setNewWater(this.gameObject);
                try
                {
                    FightingSceneStore.obstacleList.Add(pos, obstacle);
                }
                catch
                {
                    Destroy(obstacle);
                }
            }
        });
        return true;
    }
}
