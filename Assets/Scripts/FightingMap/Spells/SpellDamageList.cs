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
        List<Vector3Int> areaSpell = spell.getArea(spell.position, caster, obstacleList, tilemap);
        // Friends take damage
        foreach (var s in playerList)
        {
            // Count number of case selected in area
            int selectedNb = areaSpell.Where(x => x.Equals(s.Key.position)).Count();
            if (selectedNb > 0)
            {
                s.Key.takeDamage(spell.damage * selectedNb);
            }
        }
        // Enemies take damage
        foreach (var s in enemyList)
        {
            // Count number of case selected in area
            int selectedNb = areaSpell.Where(x => x.Equals(s.Key.position)).Count();
            if (selectedNb > 0)
            {
                s.Key.takeDamage(spell.damage * selectedNb);
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
                obs.takeDamage(spell.damage * selectedNb);
            }
        }
    }
}
