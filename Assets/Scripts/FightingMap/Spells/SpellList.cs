using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public static class SpellList
{
    private const string PATH = "Spells/";

    public static List<GameObject> getSpellList()
    {
        string nameSpell;

        ////// Spells //////
        // Fire
        nameSpell = "Fireball";
        GameObject Fireball = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        nameSpell = "Explosion";
        GameObject Explosion = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        // Wind
        nameSpell = "Tornado";
        GameObject Tornado = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        // Water
        nameSpell = "Icycle";
        GameObject Icycle = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        // Earth
        nameSpell = "Sandwall";
        GameObject Sandwall = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        // Dark
        nameSpell = "Teleportation";
        GameObject Teleportation = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        nameSpell = "Blackhole";
        GameObject Blackhole = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        /// Mix ///
        // Fire + Earth
        nameSpell = "Grogoulem";
        GameObject Grogoulem = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        ////// Attacks //////
        nameSpell = "Slash";
        GameObject Slash = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        nameSpell = "Slam";
        GameObject Slam = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        return new List<GameObject>(){
            Fireball,
            Explosion,
            Tornado,
            Icycle,
            Sandwall,
            Blackhole,
            Teleportation,
            Grogoulem,
            Slash,
            Slam
        };
    }
}
