using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class SpellList : MonoBehaviour
{
    private const string PATH = "Spells/";
    public GameObject Fireball,
    Meteor,
    Teleportation,
    Icycle,
    Sandwall,
    Slash,
    Explosion;

    /*
    public Spell
        Blackhole,
        ;
    */

    void Start()
    {
        string nameSpell;

        // Spells
        nameSpell = "Fireball";
        Fireball = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        nameSpell = "Meteor";
        Meteor = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        nameSpell = "Teleportation";
        Teleportation = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        nameSpell = "Icycle";
        Icycle = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        nameSpell = "Sandwall";
        Sandwall = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        nameSpell = "Explosion";
        Explosion = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

        // Attacks
        nameSpell = "Slash";
        Slash = Resources.Load<GameObject>(PATH + nameSpell + "/" + nameSpell);

    }
}
