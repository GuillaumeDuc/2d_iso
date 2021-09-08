using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public static class SpellList
{
    private const string PATH = "Spells/";

    ////// Spells //////
    // Fire
    static GameObject Fireball = Resources.Load<GameObject>(PATH + "Fireball" + "/" + "Fireball");
    static GameObject Explosion = Resources.Load<GameObject>(PATH + "Explosion" + "/" + "Explosion");

    // Wind
    static GameObject Tornado = Resources.Load<GameObject>(PATH + "Tornado" + "/" + "Tornado");

    // Water
    static GameObject Icycle = Resources.Load<GameObject>(PATH + "Icycle" + "/" + "Icycle");

    // Earth
    static GameObject Sandwall = Resources.Load<GameObject>(PATH + "Sandwall" + "/" + "Sandwall");

    // Dark
    static GameObject Teleportation = Resources.Load<GameObject>(PATH + "Teleportation" + "/" + "Teleportation");
    static GameObject Blackhole = Resources.Load<GameObject>(PATH + "Blackhole" + "/" + "Blackhole");

    /// Mix ///
    // Fire + Fire
    static GameObject Fireburst = Resources.Load<GameObject>(PATH + "Fireburst" + "/" + "Fireburst");
    // Fire + Earth
    static GameObject Grogoulem = Resources.Load<GameObject>(PATH + "Grogoulem" + "/" + "Grogoulem");

    ////// Attacks //////
    static GameObject Slash = Resources.Load<GameObject>(PATH + "Slash" + "/" + "Slash");
    static GameObject Slam = Resources.Load<GameObject>(PATH + "Slam" + "/" + "Slam");

    public static List<GameObject> getSpellList()
    {
        return new List<GameObject>(){
            Fireball,
            Explosion,
            Tornado,
            Icycle,
            Sandwall,
            Blackhole,
            Teleportation,
            Slash,
            Slam
        };
    }

    public static List<GameObject> getMixedSpellList()
    {
        return new List<GameObject>(){
            Grogoulem,
            Fireburst
        };
    }

    public static GameObject getMixedSpell(SpellType type)
    {
        return getMixedSpellList().Find(spell =>
        {
            Spell spellScript = spell.GetComponent<Spell>();
            return spellScript.type == type;
        });
    }
}
