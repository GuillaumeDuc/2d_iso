using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public static class SpellDamageList
{
    public static void doDamage(
        Spell spell,
        Unit caster,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
    )
    {
        List<Vector3Int> areaSpell = spell.getArea(caster, obstacleList, tilemap);
        Dictionary<Unit, GameObject> deadPlayerList = new Dictionary<Unit, GameObject>();
        Dictionary<Unit, GameObject> deadEnemyList = new Dictionary<Unit, GameObject>();
        Dictionary<Vector3Int, GameObject> destroyedObstacleList = new Dictionary<Vector3Int, GameObject>();

        // Friends take damage
        foreach (var s in playerList)
        {
            // Count number of case selected in area
            int selectedNb = areaSpell.Where(x => x.Equals(s.Key.position)).Count();
            if (selectedNb > 0)
            {
                bool isDead = s.Key.takeDamage(spell.damage * selectedNb);
                if (isDead)
                {
                    deadPlayerList.Add(s.Key, s.Value);
                }
            }
        }
        // Enemies take damage
        foreach (var s in enemyList)
        {
            // Count number of case selected in area
            int selectedNb = areaSpell.Where(x => x.Equals(s.Key.position)).Count();
            if (selectedNb > 0)
            {
                bool isDead = s.Key.takeDamage(spell.damage * selectedNb);
                if (isDead)
                {
                    deadEnemyList.Add(s.Key, s.Value);
                }
            }
        }
        // Obstacles take damage
        foreach (var o in obstacleList)
        {
            // Count number of case selected in area
            int selectedNb = areaSpell.Where(x => x.Equals(o.Key)).Count();
            if (selectedNb > 0)
            {
                Obstacle obs = o.Value.GetComponent<Obstacle>();
                bool isDestroyed = obs.takeDamage(spell.damage * selectedNb);
                if (isDestroyed)
                {
                    destroyedObstacleList.Add(o.Key, o.Value);
                }
            }
        }
    }
}
