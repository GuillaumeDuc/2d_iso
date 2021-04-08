using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Spell
{
    public GameObject spellGO;
    public Sprite sprite;
    string nameSpell;
    public int range, area, damage;
    public bool lineOfSight, selectableArea;

    public System.Action<Spell, Vector3Int, Vector3Int, Tilemap> animate { get; set; }

    public System.Func<Spell, Vector3Int, Vector3Int, Tilemap, List<Vector3Int>> getAreaList;

    public List<Vector3Int> getArea(Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        return getAreaList(this, to, from, tilemap);
    }

    public void playAnimation(Vector3Int to, Tilemap tilemap)
    {
        animate(this, to, new Vector3Int(), tilemap);
    }

    public void playAnimation(Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        animate(this, to, from, tilemap);
    }

    public Spell Explosion;

    public Spell(GameObject spellGO, string nameSpell, int damage, int range, int area = 0, bool lineOfSight = true, bool selectableArea = false)
    {
        this.spellGO = spellGO;
        this.sprite = spellGO.GetComponent<SpriteRenderer>().sprite;
        this.nameSpell = nameSpell;
        this.range = range;
        this.area = area;
        this.damage = damage;
        this.lineOfSight = lineOfSight;
        this.selectableArea = selectableArea;
    }

    override public string ToString()
    {
        return "spell go : " + spellGO + "\n" + "name : " + nameSpell + "\n" + "damage|range|area : " + damage + "|" + range + "|" + area;
    }
}
