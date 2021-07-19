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
    Sandwall;

    /*
    public Spell
        Explosion,
        Blackhole,
        Slash,
        ;
    */

    void Start()
    {
        string nameSpell;

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
    }

    /*
        // Animate Caster
        public void animateCasterAttack(Spell spell, Unit caster)
        {
            Animator animator = caster.unitGO.GetComponent<Animator>();
            string paramName = "Attack";
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == paramName)
                {
                    animator.SetTrigger(paramName);
                }
            }
        }
    */
}
