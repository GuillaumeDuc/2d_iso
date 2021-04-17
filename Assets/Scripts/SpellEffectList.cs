using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class SpellEffectList : MonoBehaviour
{
    private Tile obstacleTile;

    public SpellEffect CreateObstacle;

    private RangeUtils RangeUtils;

    void Start()
    {
        // Instantiate Utils to get area & calculations
        RangeUtils = new RangeUtils();

        // Get obstacles
        obstacleTile = Resources.Load<Tile>("Tilemaps/CellsGrid/grid_transparent_tile");

        // Create Obstacle
        CreateObstacle = new SpellEffect();
        CreateObstacle.applyEffectAction = createObstacle;

        // Push
        //Push = new SpellEffect(1);
        // Push.applyEffectAction = pushEffect;
    }

    public void createObstacle(
        Spell spell,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        spell.getArea(obstacleList, tilemap).ForEach(c =>
        {
            try
            {
                obstacleList.Add(c, spell.spellGO);
            }
            catch
            {

            }
        });
    }
}
