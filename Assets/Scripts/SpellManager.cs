using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public GameObject Explosion;
    public GameObject Fireball;
    public GameObject Icycle;
    
    private GameObject CurrentSpell;

    void launchSpell(GameObject Spell, Vector2 pos)
    {
        Instantiate(Spell, pos, Quaternion.identity);
    }

    public void Spell1Clicked()
    {
        CurrentSpell = Icycle;
    }

    void Start()
    {
        CurrentSpell = Explosion;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            launchSpell(CurrentSpell, worldPosition);
        }
    }
}
