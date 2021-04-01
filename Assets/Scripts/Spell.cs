using UnityEngine.Tilemaps;
using UnityEngine;

public class Spell
{
    public GameObject spellGO;
    public Sprite sprite;
    string nameSpell;
    int range, area, damage;

    public System.Action<Spell, Vector3Int, Vector3Int, Tilemap> animate { get; set; }

    public void playAnimation(Vector3Int to, Tilemap tilemap)
    {
        animate(this, to, new Vector3Int(), tilemap);
    }

    public void playAnimation(Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        animate(this, to, from, tilemap);
    }

    public Spell Explosion;

    public Spell(GameObject spellGO, string nameSpell, int range, int area, int damage)
    {
        this.spellGO = spellGO;
        this.sprite = spellGO.GetComponent<SpriteRenderer>().sprite;
        this.nameSpell = nameSpell;
        this.range = range;
        this.area = area;
        this.damage = damage;
    }

    override public string ToString()
    {
        return "spell go : " + spellGO + "\n" + "name : " + nameSpell + "\n" + "range|area|damage : " + range + "|" + area + "|" + damage;
    }
}
