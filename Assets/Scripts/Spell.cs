using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Spell
{
    public GameObject spellGO;
    public Sprite sprite;
    string nameSpell;
    public int range, area, damage, clickNb;
    public bool lineOfSight;
    public List<Vector3Int> spellPos;
    public Vector3Int casterPos;

    public System.Action<Spell, Tilemap> animate { get; set; }

    public System.Func<Spell, Tilemap, List<Vector3Int>> getAreaList;

    public System.Func<Spell, Tilemap, List<Vector3Int>> getRangeList;

    public List<Vector3Int> getRange(Tilemap tilemap)
    {
        return getRangeList(this, tilemap);
    }

    public List<Vector3Int> getArea(Tilemap tilemap)
    {
        return getAreaList(this, tilemap);
    }

    public void playAnimation(Tilemap tilemap)
    {
        animate(this, tilemap);
    }

    public Spell(GameObject spellGO, string nameSpell, int damage, int range, int area = 0, bool lineOfSight = true, int clickNb = 1)
    {
        this.spellGO = spellGO;
        this.sprite = spellGO.GetComponent<SpriteRenderer>().sprite;
        this.nameSpell = nameSpell;
        this.range = range;
        this.area = area;
        this.damage = damage;
        this.lineOfSight = lineOfSight;
        this.clickNb = clickNb;
        spellPos = new List<Vector3Int>();
    }

    override public string ToString()
    {
        return "spell go : " + spellGO + "\n" + "name : " + nameSpell + "\n" + "damage|range|area : " + damage + "|" + range + "|" + area;
    }
}
