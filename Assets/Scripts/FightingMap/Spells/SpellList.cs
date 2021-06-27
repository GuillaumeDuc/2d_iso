using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class SpellList : MonoBehaviour
{
    private const string PATH = "Spells/";

    public SpellEffectList SpellEffectList;

    public Spell
        Explosion,
        Icycle,
        Sandwall,
        Blackhole,
        Teleportation,
        Slash,
        Meteor,
        Fireball
        ;

    private string nameSpell;

    void Start()
    {
        // Explosion
        nameSpell = "Explosion";
        GameObject ExplosionGO = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);
        // GameObject, name, damage, range, area, line of sight, click nb, unique cell area
        Explosion = new Spell(ExplosionGO, nameSpell, 10, 10, 1, true, 3, false, 50);
        Explosion.getRangeList = getRangeInCircleFullPlayer;
        Explosion.getAreaList = getAreaInCircleFull;
        Explosion.instantiateAction = instatiateAreaWithDelay;
        Explosion.canCastOn = canCast;
        // Effects
        Explosion.spellEffectList.Add(SpellEffectList.Fire);
        // Damage
        Explosion.doDamageAction = doDamage;

        // Icycle
        nameSpell = "Icycle";
        GameObject IcycleGO = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);
        Icycle = new Spell(IcycleGO, nameSpell, 5, 10, 1, false, 5, false, 50);
        Icycle.getRangeList = getRangeInCircleFullPlayer;
        Icycle.getAreaList = getAreaInCircleFull;
        Icycle.instantiateAction = instantiateOnCellClicked;
        Icycle.canCastOn = canCast;
        // Effects
        Icycle.spellEffectList.Add(SpellEffectList.Freeze);
        // Damage
        Icycle.doDamageAction = doDamage;

        // Sandwall
        nameSpell = "Sandwall";
        GameObject SandwallGO = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);
        Sandwall = new Spell(SandwallGO, nameSpell, 0, 10, 1, false, 2, true, 100);
        Sandwall.getRangeList = getRangeInCircleFullPlayer;
        Sandwall.getAreaList = getAreaInLineBetweenCells;
        // Walls are instantiated and modified here
        Sandwall.instantiateAction = instantiateObstacles;
        Sandwall.canCastOn = canCast;
        //Add effect on spell
        Sandwall.spellEffectList.Add(SpellEffectList.PushFromPlayer);
        // Damage
        Sandwall.doDamageAction = doDamage;

        // Blackhole
        nameSpell = "Blackhole";
        GameObject BlackholeGO = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);
        Blackhole = new Spell(BlackholeGO, nameSpell, 90, 5, 0, true, 1, false, 70);
        Blackhole.getRangeList = getRangeInCircleFullPlayer;
        Blackhole.getAreaList = getAreaInCircleFull;
        Blackhole.instantiateAction = instantiateOnCellClicked;
        Blackhole.canCastOn = canCast;
        // Effects
        // Damage
        Blackhole.doDamageAction = doDamage;

        // Teleportation
        nameSpell = "Teleportation";
        GameObject TeleportationGO = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);
        Teleportation = new Spell(TeleportationGO, nameSpell, 0, 10, 0, false, 1, false, 30);
        Teleportation.getRangeList = getRangeInCircleFullPlayer;
        Teleportation.getAreaList = getAreaInCircleFull;
        Teleportation.canCastOn = canCast;
        // Effects
        Teleportation.spellEffectList.Add(SpellEffectList.Teleport);
        // Damage

        // Slash
        nameSpell = "Slash";
        GameObject SlashGO = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);
        Slash = new Spell(SlashGO, nameSpell, 50, 1, 0, false, 1, false, 50);
        Slash.getRangeList = getRangeInCircleFullPlayer;
        Slash.getAreaList = getAreaInCircleFull;
        Slash.canCastOn = canCast;
        // Effects
        // Damage
        Slash.doDamageAction = doDamage;
        // Animate Caster
        Slash.animateCasterAction = animateCasterAttack;

        // Meteor
        nameSpell = "Meteor";
        GameObject MeteorGO = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);
        Meteor = new Spell(MeteorGO, nameSpell, 100, 15, 4, false, 1, false, 1, true);
        Meteor.getRangeList = getRangeInCircleFullPlayer;
        Meteor.getAreaList = getAreaAndresCircle;
        Meteor.instantiateAction = instantiateOnCellClicked;
        Meteor.canCastOn = canCast;
        // Damage
        Meteor.doDamageAction = doDamage;
        // Effects
        Meteor.spellEffectList.Add(SpellEffectList.BurnTile);
        // Delay
        Meteor.delayEffect = 6.7f;
        Meteor.delayDamage = 4.1f;

        // Fireball
        nameSpell = "Fireball";
        GameObject FireballGO = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);
        Fireball = new Spell(FireballGO, nameSpell, 100, 15, 3, false, 1, false, 1, true);
        Fireball.getRangeList = getRangeInCircleFullPlayer;
        Fireball.getAreaList = getAreaInCircleFull;
        Fireball.instantiateAction = instantiateThrowedSpell;
        Fireball.canCastOn = canCast;
        // Damage
        Fireball.doDamageAction = doDamage;
        // Effects
        Fireball.spellEffectList.Add(SpellEffectList.Fire);
    }

    public bool canCast(
        Spell spell,
        Unit caster,
        List<Vector3Int> range,
        Vector3Int cell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        // Tile is empty
        if (!tilemap.HasTile(cell))
        {
            return false;
        }
        // No spell selected
        if (spell == null)
        {
            return false;
        }
        // Check line of sight
        if (spell.lineOfSight &&
            !RangeUtils.lineOfSight(
                caster.position,
                cell,
                obstacleList,
                tilemap
                )
            )
        {
            return false;
        }
        // Check if cell is in range
        if (!range.Contains(cell))
        {
            return false;
        }
        return true;
    }

    public bool canCast(
        Spell spell,
        Unit caster,
        Vector3Int cell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        return canCast(spell, caster, spell.getRange(caster, obstacleList, tilemap), cell, obstacleList, tilemap);
    }

    // Instantiation
    public void instatiateAreaWithDelay(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> listCells = spell.getArea(caster, obstacleList, tilemap);

        StartCoroutine(multipleInstantiateOnCell(listCells, spell, obstacleList, tilemap));
    }

    IEnumerator multipleInstantiateOnCell(
        List<Vector3Int> listCells,
        Spell spell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        foreach (var c in listCells)
        {
            yield return new WaitForSeconds(0.1f);
            instantiateOnOneCell(spell, c, tilemap);
        }
    }

    public void instantiateOnOneCell(
        Spell spell,
        Vector3Int to,
        Tilemap tilemap
        )
    {
        Vector2 worldPos = tilemap.CellToWorld(to);
        // Instantiate animation
        Instantiate(spell.spellGO, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
    }

    public void instantiateOnCellClicked(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> listCells = spell.getArea(caster, obstacleList, tilemap);
        spell.spellPos.ForEach(v =>
        {
            instantiateOnOneCell(spell, v, tilemap);
        });
    }

    public void instantiateObstacles(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> listCells = spell.getArea(caster, obstacleList, tilemap);
        StartCoroutine(delayInstantiateObstacle(listCells, spell.spellGO, obstacleList, tilemap));
    }

    IEnumerator delayInstantiateObstacle(
        List<Vector3Int> area,
        GameObject spellGO,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        foreach (var pos in area)
        {
            yield return new WaitForSeconds(0.1f);
            Vector2 worldPos = tilemap.CellToWorld(pos);
            GameObject obstacle = Instantiate(spellGO, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
            try
            {
                obstacleList.Add(pos, obstacle);
            }
            catch
            {
                Destroy(obstacle);
            }
        };
    }

    public void instantiateThrowedSpell(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        
    }

    // Range 
    public List<Vector3Int> getRangeInCircleFullPlayer(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();
        // Get full circle
        List<Vector3Int> listSquare = RangeUtils.getAreaCircleFull(caster.position, spell.range, tilemap);

        // Check for each square if spell can be cast
        listSquare.ForEach(s =>
        {
            // Spell is always in range though
            if (canCast(spell, caster, listSquare, s, obstacleList, tilemap))
            {
                area.Add(s);
            }
        });

        return area;
    }

    // Area of Effect
    public List<Vector3Int> getAreaInCircleFull(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();

        spell.spellPos.ForEach(s =>
        {
            area = area.Concat(RangeUtils.getAreaCircleFull(s, spell.area, tilemap)).ToList();
        });
        return area;
    }

    public List<Vector3Int> getAreaInLine(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();

        spell.spellPos.ForEach(s =>
        {
            area = area.Concat(RangeUtils.getAreaInLine(caster.position, s, obstacleList, tilemap, spell.uniqueCellArea)).ToList();
        });
        return area;
    }

    public List<Vector3Int> getAreaInLineBetweenCells(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();

        for (int i = 0; i < spell.spellPos.Count() - 1; i++)
        {
            area = area.Concat(
                RangeUtils.getAreaInLine(
                    spell.spellPos[i],
                    spell.spellPos[i + 1],
                    obstacleList,
                    tilemap,
                    spell.uniqueCellArea
                    )
                ).ToList();
        }
        return area;
    }

    public List<Vector3Int> getAreaAndresCircle(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();

        spell.spellPos.ForEach(s =>
        {
            Vector3Int current = s;
            List<Vector3Int> circle = RangeUtils.AndresCircle(current.x, current.y, spell.area);

            if (spell.burst)
            {
                // If cell selected equals an obstacle move spellpos one cell toward player
                if (obstacleList.ContainsKey(current))
                {
                    current = RangeUtils.getClosestNeighbour(current, caster.position, tilemap);
                }

                List<Vector3Int> newCircle = new List<Vector3Int>();
                circle.ForEach(v =>
                {
                    if (RangeUtils.lineOfSight(current, v, obstacleList, tilemap))
                    {
                        newCircle.Add(v);
                    }
                });
                circle = newCircle;
            }
            area = area.Concat(circle).ToList();
        });

        return area;
    }

    // Damage
    public void doDamage(
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
        // Remove dead
        foreach (var s in deadPlayerList)
        {
            playerList.Remove(s.Key);
            Destroy(s.Value);
        }
        foreach (var s in deadEnemyList)
        {
            enemyList.Remove(s.Key);
            Destroy(s.Value);
        }
        foreach (var s in destroyedObstacleList)
        {
            obstacleList.Remove(s.Key);
            DestroyImmediate(s.Value);
        }
    }

    // Animate Caster
    public void animateCasterAttack(Spell spell, Unit caster)
    {
        Animator animator = caster.unitGO.GetComponent<Animator>();
        string paramName = "Attack";
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
            {
                animator.SetTrigger(paramName);
            }
        }
    }
}
