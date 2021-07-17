using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Spell : MonoBehaviour
{
    [HideInInspector]
    public string nameSpell;
    public int range, area, damage, clickNb, manaCost;
    public bool lineOfSight, uniqueCellArea, burst;

    [HideInInspector]
    public Vector3Int position;

    //[HideInInspector]
    //public List<Vector3Int> spellPos;

    [HideInInspector]
    public Unit caster;

    // Area
    public enum FunctionArea
    {
        Circle,
        InLine,
        InLineBetweenCells,
        AndresCircle
    };

    public FunctionArea selectedArea;
    private Dictionary<FunctionArea, System.Func<Spell, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>>> functionAreaLookup = new Dictionary<FunctionArea, System.Func<Spell, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>>>()
        {
            { FunctionArea.Circle, SpellAreaList.getAreaInCircleFull },
            { FunctionArea.InLine, SpellAreaList.getAreaInLine },
            { FunctionArea.InLineBetweenCells, SpellAreaList.getAreaInLineBetweenCells },
            { FunctionArea.AndresCircle, SpellAreaList.getAreaAndresCircle },
        };


    // Range
    public enum FunctionRange
    {
        Circle
    };

    public FunctionRange selectedRange;
    private Dictionary<FunctionRange, System.Func<Spell, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>>> functionRangeLookup = new Dictionary<FunctionRange, System.Func<Spell, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>>>()
        {
            { FunctionRange.Circle, SpellRangeList.getRangeInCircleFull },
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
        InstatiateAreaWithDelay,
        InstantiateOnCellClicked,
        InstantiateObstacles,
        InstantiateThrowedSpell
    };

    public FunctionInstantiate selectedInstantiate;
    static SpellInstantiateList si = new SpellInstantiateList();
    Dictionary<Spell.FunctionInstantiate, System.Action<Spell, Unit, Vector3Int, Dictionary<Vector3Int, GameObject>, Tilemap>> functionInstantiateLookup = new Dictionary<Spell.FunctionInstantiate, System.Action<Spell, Unit, Vector3Int, Dictionary<Vector3Int, GameObject>, Tilemap>>()
        {
            { Spell.FunctionInstantiate.InstatiateAreaWithDelay, si.instantiateAreaWithDelay },
            { Spell.FunctionInstantiate.InstantiateOnCellClicked, si.instantiateOnCellClicked },
            { Spell.FunctionInstantiate.InstantiateObstacles, si.instantiateObstacles },
            { Spell.FunctionInstantiate.InstantiateThrowedSpell, si.instantiateThrowedSpell },
        };


    // Lookup functions
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

    public void instantiateSpell(Unit caster, Vector3Int target, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        functionInstantiateLookup[selectedInstantiate].Invoke(this, caster, target, obstacleList, tilemap);
    }

    public void setSpell(Spell spell)
    {
        nameSpell = spell.nameSpell;
        range = spell.range;
        area = spell.area;
        damage = spell.damage;
        clickNb = spell.clickNb;
        manaCost = spell.manaCost;
        lineOfSight = spell.lineOfSight;
        uniqueCellArea = spell.uniqueCellArea;
        burst = spell.burst;
        position = spell.position;
        //spellPos = new List<Vector3Int>(spell.spellPos);
        caster = spell.caster;
        selectedArea = spell.selectedArea;
        selectedRange = spell.selectedRange;
        selectedCanCast = spell.selectedCanCast;
        selectedInstantiate = spell.selectedInstantiate;
    }

    override public string ToString()
    {
        return "name : " + nameSpell + "\n" +
            "damage|range|area : " + damage + "|" + range + "|" + area + "\n" +
            "line of sight : " + lineOfSight;
    }
}
