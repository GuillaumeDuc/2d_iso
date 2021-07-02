using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public string nameSpell;
    public int range, area, damage, clickNb, manaCost;
    public float delayEffect, delayDamage;
    public bool lineOfSight, uniqueCellArea, burst;
    public List<Vector3Int> spellPos;
    public List<SpellEffect> spellEffectList;

    public Unit caster;

    // Area
    private enum FunctionArea
    {
        Circle,
        InLine,
        InLineBetweenCells,
        AndresCircle
    };

    [SerializeField]
    private FunctionArea selectedArea;
    private Dictionary<FunctionArea, System.Func<Spell, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>>> functionAreaLookup;

    // Range
    private enum FunctionRange
    {
        Circle
    };

    [SerializeField]
    private FunctionRange selectedRange;
    private Dictionary<FunctionRange, System.Func<Spell, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>>> functionRangeLookup;

    // Can Cast
    private enum FunctionCanCast
    {
        CanCast
    };

    [SerializeField]
    private FunctionCanCast selectedCanCast;
    private Dictionary<FunctionCanCast, System.Func<Spell, Unit, List<Vector3Int>, Vector3Int, Dictionary<Vector3Int, GameObject>, Tilemap, bool>> functionCanCastLookup;

    private void Awake()
    {
        functionAreaLookup = new Dictionary<FunctionArea, System.Func<Spell, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>>>()
        {
            { FunctionArea.Circle, SpellAreaList.getAreaInCircleFull },
            { FunctionArea.InLine, SpellAreaList.getAreaInLine },
            { FunctionArea.InLineBetweenCells, SpellAreaList.getAreaInLineBetweenCells },
            { FunctionArea.AndresCircle, SpellAreaList.getAreaAndresCircle },
        };

        functionRangeLookup = new Dictionary<FunctionRange, System.Func<Spell, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>>>()
        {
            { FunctionRange.Circle, SpellRangeList.getRangeInCircleFull},
        };

        functionCanCastLookup = new Dictionary<FunctionCanCast, System.Func<Spell, Unit, List<Vector3Int>, Vector3Int, Dictionary<Vector3Int, GameObject>, Tilemap, bool>>()
        {
            { FunctionCanCast.CanCast, SpellCanCastList.canCast},
        };
    }

    public List<Vector3Int> getArea(Unit caster, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        return functionAreaLookup[selectedArea].Invoke(this, caster, obstacleList, tilemap);
    }

    public List<Vector3Int> getRange(Unit caster, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        return functionRangeLookup[selectedRange].Invoke(this, caster, obstacleList, tilemap);
    }

    public bool canCast(Unit caster, List<Vector3Int> range, Vector3Int target, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        return functionCanCastLookup[selectedCanCast].Invoke(this, caster, range, target, obstacleList, tilemap);
    }

    public void applyEffect(
        Unit caster,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        spellEffectList.ForEach(se =>
        {
            se.applyEffect(this, caster, playerList, enemyList, obstacleList, tilemap);
        });
    }

    override public string ToString()
    {
        return "name : " + nameSpell + "\n" +
            "damage|range|area : " + damage + "|" + range + "|" + area + "\n" +
            "line of sight : " + lineOfSight;
    }
}
