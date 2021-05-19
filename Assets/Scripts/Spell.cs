using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Spell
{
    public GameObject spellGO;
    public Sprite sprite;
    string nameSpell;
    public int range, area, damage, clickNb, manaCost;
    public bool lineOfSight, uniqueCellArea;
    public List<Vector3Int> spellPos;
    public Vector3Int casterPos;
    public List<SpellEffect> spellEffectList;

    public System.Action<Spell, Dictionary<Vector3Int, GameObject>, Tilemap> animate { get; set; }
    public System.Func<Spell, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>> getAreaList;
    public System.Func<Spell, Dictionary<Vector3Int, GameObject>, Tilemap, List<Vector3Int>> getRangeList;
    public System.Func<Spell, Vector3Int, Dictionary<Vector3Int, GameObject>, Tilemap, bool> canCastOn;
    public System.Action<Spell, Dictionary<Unit, GameObject>, Dictionary<Unit, GameObject>, Dictionary<Vector3Int, GameObject>, Tilemap> doDamageAction;

    public void doDamage(
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        doDamageAction(this, playerList, enemyList, obstacleList, tilemap);
    }

    public bool canCast(Vector3Int cell, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        return canCastOn(this, cell, obstacleList, tilemap);
    }

    public List<Vector3Int> getRange(Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        return getRangeList(this, obstacleList, tilemap);
    }

    public List<Vector3Int> getArea(Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        return getAreaList(this, obstacleList, tilemap);
    }

    public void playAnimation(Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        animate?.Invoke(this, obstacleList, tilemap);
    }

    public void applyEffect(
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        spellEffectList.ForEach(se =>
        {
            se.applyEffect(this, playerList, enemyList, obstacleList, tilemap);
        });
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
        int manaCost = 10
        )
    {
        this.spellGO = spellGO;
        this.sprite = spellGO.GetComponent<SpriteRenderer>().sprite;
        this.nameSpell = nameSpell;
        this.range = range;
        this.area = area;
        this.damage = damage;
        this.lineOfSight = lineOfSight;
        this.clickNb = clickNb;
        this.uniqueCellArea = uniqueCellArea;
        this.manaCost = manaCost;
        spellPos = new List<Vector3Int>();
        spellEffectList = new List<SpellEffect>();
    }

    override public string ToString()
    {
        return "spell go : " + spellGO + "\n" +
            "name : " + nameSpell + "\n" +
            "damage|range|area : " + damage + "|" + range + "|" + area + "\n" +
            "line of sight : " + lineOfSight;
    }
}
