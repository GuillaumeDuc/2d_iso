using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public GameObject Explosion;
    public GameObject Fireball;
    public GameObject Icycle;
    public GameObject Lightning;
    private int spellNb = 0;
    List<GameObject> spellList = new List<GameObject> ();
    private GameObject CurrentSpell;

    void launchSpell(GameObject Spell, Vector2 pos)
    {
        Instantiate(Spell, pos, Quaternion.identity);
    }

    public void Spell1Clicked()
    {
        CurrentSpell = spellList[spellNb];
        spellNb += 1;
        if (spellNb > spellList.Count - 1)
        {
            spellNb = 0;
        }
    }

    void Start()
    {
        spellList.InsertRange(spellList.Count, new GameObject[] { Explosion, Fireball, Icycle, Lightning });
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            if(CurrentSpell!=null)
            launchSpell(CurrentSpell, worldPosition);
        }
    }
}
