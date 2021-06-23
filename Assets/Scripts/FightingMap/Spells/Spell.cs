using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Spell
{
    public GameObject spellGO;
    public Sprite sprite;
    string nameSpell;
    public int range, area, damage, clickNb, manaCost;
    public float delayEffect, delayDamage;
    public bool lineOfSight, uniqueCellArea, burst;
    public List<Vector3Int> spellPos;
    public List<SpellEffect> spellEffectList;

    public System.Action<Spell, Unit, Dictionary<Vector3Int, GameObject>, Tilemap> animate { get; set; }
    public System.Func<Spell, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>> getAreaList;
    public System.Func<Spell, Unit, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>> getRangeList;
    public System.Func<Spell, Unit, Vector3Int, Dictionary<Vector3Int, GameObject>, Tilemap, bool> canCastOn;
    public System.Action<Spell, Unit, Dictionary<Unit, GameObject>, Dictionary<Unit, GameObject>, Dictionary<Vector3Int, GameObject>, Tilemap> doDamageAction;
    public System.Action<Spell, Unit> animateCasterAction;

    public void doDamage(
        Unit caster,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        doDamageAction?.Invoke(this, caster, playerList, enemyList, obstacleList, tilemap);
    }

    public void animateCaster(Unit caster)
    {
        animateCasterAction?.Invoke(this, caster);
    }

    public bool canCast(Unit caster, Vector3Int cell, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        return canCastOn(this, caster, cell, obstacleList, tilemap);
    }

    public List<Vector3Int> getRange(Unit caster, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        return getRangeList(this, caster, obstacleList, tilemap);
    }

    public List<Vector3Int> getArea(Unit caster, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        return getAreaList(this, caster, obstacleList, tilemap);
    }

    public void playAnimation(Unit caster, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        animate?.Invoke(this, caster, obstacleList, tilemap);
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

    public Spell(Spell spell)
    {
        spellGO = spell.spellGO;
        sprite = spell.sprite;
        nameSpell = spell.nameSpell;
        range = spell.range;
        area = spell.area;
        damage = spell.damage;
        lineOfSight = spell.lineOfSight;
        burst = spell.burst;
        clickNb = spell.clickNb;
        uniqueCellArea = spell.uniqueCellArea;
        manaCost = spell.manaCost;
        spellPos = new List<Vector3Int>(spell.spellPos);
        spellEffectList = new List<SpellEffect>(spell.spellEffectList);
        delayEffect = spell.delayEffect;
        delayDamage = spell.delayDamage;

        animate = spell.animate;
        getAreaList = spell.getAreaList;
        getRangeList = spell.getRangeList;
        canCastOn = spell.canCastOn;
        doDamageAction = spell.doDamageAction;
        animateCasterAction = spell.animateCasterAction;
    }

    public Spell(
        GameObject spellGO,
        string nameSpell,
        int damage,
        int range,
        int area = 0,
        bool lineOfSight = true,
        int clickNb = 1,
        bool uniqueCellArea = false,
        int manaCost = 10,
        bool burst = false
        )
    {
        this.spellGO = spellGO;
        this.sprite = spellGO.GetComponent<SpriteRenderer>().sprite;
        this.nameSpell = nameSpell;
        this.range = range;
        this.area = area;
        this.damage = damage;
        this.lineOfSight = lineOfSight;
        this.burst = burst;
        this.clickNb = clickNb;
        this.uniqueCellArea = uniqueCellArea;
        this.manaCost = manaCost;
        spellPos = new List<Vector3Int>();
        spellEffectList = new List<SpellEffect>();
        delayEffect = 0;
    }

    override public string ToString()
    {
        return "spell go : " + spellGO + "\n" +
            "name : " + nameSpell + "\n" +
            "damage|range|area : " + damage + "|" + range + "|" + area + "\n" +
            "line of sight : " + lineOfSight;
    }
}
