using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpellList : MonoBehaviour
{
    private const string PATH = "Spells/SpellsPrefab/";

    public Spell Explosion, Icycle;
    private static GameObject ExplosionGO, IcycleGO;
    private string nameSpell;

    void Start()
    {
        // Explosion
        nameSpell = "Explosion";
        ExplosionGO = Resources.Load<GameObject>(PATH + nameSpell);
        Explosion = new Spell(ExplosionGO, nameSpell, 3, 1, 10);
        Explosion.animate = animateOnCell;

        // Icycle
        nameSpell = "Icycle";
        IcycleGO = Resources.Load<GameObject>(PATH + nameSpell);
        Icycle = new Spell(IcycleGO, nameSpell, 3, 1, 10);
        Icycle.animate = animateInLine;
    }

    // Animation

    public void animateOnCell(Spell spell, Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        Vector2 worldPos = tilemap.CellToWorld(to);
        Instantiate(spell.spellGO, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
    }

    public void animateInLine(Spell spell, Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        Vector2 worldPosTo = tilemap.CellToWorld(to);
        Vector3Int current = new Vector3Int(to.x, to.y, to.z);
        List<Vector3Int> listCells = new List<Vector3Int>();

        while (current != from)
        {
            listCells.Add(current);
            current = getClosestNeighbour(current, from, tilemap);
        }
        
        listCells.Reverse();

        foreach (var c in listCells)
        {
            animateOnCell(spell, c, from, tilemap);
        }

    }

    private Vector3Int getClosestNeighbour(Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        Vector3Int up = new Vector3Int(to.x, to.y + 1, to.z);
        Vector3Int down = new Vector3Int(to.x, to.y - 1, to.z);
        Vector3Int left = new Vector3Int(to.x - 1, to.y, to.z);
        Vector3Int right = new Vector3Int(to.x + 1, to.y, to.z);
        Vector3Int topRight = new Vector3Int(to.x + 1, to.y + 1, to.z);
        Vector3Int topLeft = new Vector3Int(to.x - 1, to.y + 1, to.z);
        Vector3Int bottomRight = new Vector3Int(to.x + 1, to.y - 1, to.z);
        Vector3Int bottomLeft = new Vector3Int(to.x - 1, to.y - 1, to.z);

        Dictionary<Vector3Int, float> neighbours = new Dictionary<Vector3Int, float>() {
            { up, getMDistance(up, from) },
            { down, getMDistance(down, from) },
            { left, getMDistance(left, from) },
            { right, getMDistance(right, from) },
            { topRight, getMDistance(topRight, from) },
            { topLeft, getMDistance(topLeft, from) },
            { bottomRight, getMDistance(bottomRight, from) },
            {bottomLeft, getMDistance(bottomLeft, from) },
        };

        return neighbours.FirstOrDefault(x => x.Value == neighbours.Min(n => n.Value)).Key;
    }

    private float getMDistance(Vector3Int to, Vector3Int from)
    {
        return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
    }
}
