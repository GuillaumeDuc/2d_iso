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
    static GameObject Flaywind = Resources.Load<GameObject>(PATH + "Flaywind" + "/" + "Flaywind");
    static GameObject Windleap = Resources.Load<GameObject>(PATH + "Windleap" + "/" + "Windleap");
    static GameObject Tornado = Resources.Load<GameObject>(PATH + "Tornado" + "/" + "Tornado");

    // Water
    static GameObject Waveslash = Resources.Load<GameObject>(PATH + "Waveslash" + "/" + "Waveslash");
    static GameObject Bubble = Resources.Load<GameObject>(PATH + "Bubble" + "/" + "Bubble");
    static GameObject Waterboost = Resources.Load<GameObject>(PATH + "Waterboost" + "/" + "Waterboost");



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
    // Fire + Wind
    static GameObject Chainlightning = Resources.Load<GameObject>(PATH + "Chainlightning" + "/" + "Chainlightning");
    // Water + Wind
    static GameObject Icycle = Resources.Load<GameObject>(PATH + "Icycle" + "/" + "Icycle");

    ////// Attacks //////
    static GameObject Slash = Resources.Load<GameObject>(PATH + "Slash" + "/" + "Slash");
    static GameObject Slam = Resources.Load<GameObject>(PATH + "Slam" + "/" + "Slam");

    public static List<GameObject> getSpellList()
    {
        return new List<GameObject>(){
            Fireball,
            Explosion,
            Waveslash,
            Bubble,
            Flaywind,
            Windleap,
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
            Fireburst,
            Waterboost,
            Icycle,
            Tornado,
            Chainlightning
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
