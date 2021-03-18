using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public GameObject Explosion;
    public GameObject Fireball;
    public GameObject Icycle;

    void launchSpell(GameObject Spell, Vector2 pos)
    {
        Instantiate(Spell, pos, Quaternion.identity);
    }

    void Start()
    {
        
    }

    void Update()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(worldPosition);
            launchSpell(Icycle, worldPosition);
            // launchFireball(worldPosition);
        }
    }
}
