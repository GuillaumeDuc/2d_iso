using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public enum SpellType
{
    Fire,
    Water,
    Wind,
    Earth,
    Dark,
    Magma,
    Ice,
    Steam,
    Physical,
    Electricity
};

public class Spell : MonoBehaviour
{
    public int range = 1, area, damage, clickNb = 1, manaCost;
    public bool lineOfSight, uniqueCellArea, burst;
    public GameObject unitSpellEffect;
    public SpellType type;
    [HideInInspector]
    public Status includesOnly;

    [HideInInspector]
    public Vector3Int position;

    [HideInInspector]
    public Unit caster;

    // Area
    public enum FunctionArea
    {
        Circle,
        InLine,
        InLineBetweenCells,
        AndresCircle,
        InLineHorizontal,
        ChainEnemies
    };

    public FunctionArea selectedArea;
    private Dictionary<FunctionArea, System.Func<Spell, Vector3Int, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>>> functionAreaLookup = new Dictionary<FunctionArea, System.Func<Spell, Vector3Int, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>>>()
        {
            { FunctionArea.Circle, SpellAreaList.getAreaInCircleFull },
            { FunctionArea.InLine, SpellAreaList.getAreaInLine },
            { FunctionArea.InLineBetweenCells, SpellAreaList.getAreaInLineBetweenCells },
            { FunctionArea.AndresCircle, SpellAreaList.getAreaAndresCircle },
            { FunctionArea.InLineHorizontal, SpellAreaList.getAreaInLineHorizontal },
            { FunctionArea.ChainEnemies, SpellAreaList.getAreaChainEnemies },
        };


    // Range
    public enum FunctionRange
    {
        Circle,
        Line
    };

    public FunctionRange selectedRange;
    private Dictionary<FunctionRange, System.Func<Spell, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>>> functionRangeLookup = new Dictionary<FunctionRange, System.Func<Spell, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>>>()
        {
            { FunctionRange.Circle, SpellRangeList.getRangeInCircleFull },
            { FunctionRange.Line, SpellRangeList.getRangeInLine },
        };

    // Can Cast
    public enum FunctionCanCast
    {
        CanCast
    };

    public FunctionCanCast selectedCanCast;
    private Dictionary<FunctionCanCast, System.Func<Spell, Unit, List<Vector3Int>, Vector3Int, Dictionary<Vector3Int, GameObject>, Tilemap, bool>> functionCanCastLookup = new Dictionary<FunctionCanCast, System.Func<Spell, Unit, List<Vector3Int>, Vector3Int, Dictionary<Vector3Int, GameObject>, Tilemap, bool>>()
        {
            { FunctionCanCast.CanCast, SpellCanCastList.canCast },
        };

    // Instantiate
    public enum FunctionInstantiate
    {
        Area,
        OnCellClicked,
        Obstacles,
        ThrowedSpell,
        MoveInLineFromPlayer,
        Attack,
        AreaWithDelay
    };

    public FunctionInstantiate selectedInstantiate;
    Dictionary<Spell.FunctionInstantiate, System.Action<Spell, Unit, Vector3Int, Dictionary<Vector3Int, GameObject>, Tilemap>> functionInstantiateLookup = new Dictionary<Spell.FunctionInstantiate, System.Action<Spell, Unit, Vector3Int, Dictionary<Vector3Int, GameObject>, Tilemap>>()
        {
            { Spell.FunctionInstantiate.Area, SpellInstantiateList.instantiateArea },
            { Spell.FunctionInstantiate.OnCellClicked, SpellInstantiateList.instantiateOnCellClicked },
            { Spell.FunctionInstantiate.Obstacles, SpellInstantiateList.instantiateObstacles },
            { Spell.FunctionInstantiate.ThrowedSpell, SpellInstantiateList.instantiateThrowedSpell },
            { Spell.FunctionInstantiate.MoveInLineFromPlayer, SpellInstantiateList.instantiateMoveInLineFromPlayer },
            { Spell.FunctionInstantiate.Attack, SpellInstantiateList.instantiateAttack },
            { Spell.FunctionInstantiate.AreaWithDelay, SpellInstantiateList.instantiateAreaWithDelay },

        };


    // Lookup functions
    public List<Vector3Int> getArea(Vector3Int target, Unit caster, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        return functionAreaLookup[selectedArea].Invoke(this, target, caster, obstacleList, tilemap);
    }

    public List<Vector3Int> getRange(Unit caster, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        return functionRangeLookup[selectedRange].Invoke(this, caster, obstacleList, tilemap);
    }

    public bool canCast(Unit caster, List<Vector3Int> range, Vector3Int target, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        return functionCanCastLookup[selectedCanCast].Invoke(this, caster, range, target, obstacleList, tilemap);
    }

    public void instantiateSpell(Unit caster, Vector3Int target, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        functionInstantiateLookup[selectedInstantiate].Invoke(this, caster, target, obstacleList, tilemap);
    }

    public void setSpell(Spell spell)
    {
        range = spell.range;
        area = spell.area;
        damage = spell.damage;
        clickNb = spell.clickNb;
        manaCost = spell.manaCost;
        lineOfSight = spell.lineOfSight;
        uniqueCellArea = spell.uniqueCellArea;
        burst = spell.burst;
        position = spell.position;
        caster = spell.caster;
        selectedArea = spell.selectedArea;
        selectedRange = spell.selectedRange;
        selectedCanCast = spell.selectedCanCast;
        selectedInstantiate = spell.selectedInstantiate;
    }

    override public string ToString()
    {
        return "name : " + name + "\n" +
            "damage|range|area : " + damage + "|" + range + "|" + area + "\n" +
            "line of sight : " + lineOfSight;
    }
}
