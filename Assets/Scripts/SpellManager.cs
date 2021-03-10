using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public GameObject Explosion;
    public GameObject Fireball;

    void launchExplosion(Vector2 pos)
    {
        Instantiate(Explosion, pos, Quaternion.identity);
    }

    void launchFireball(Vector2 pos)
    {
        Instantiate(Fireball, pos, Quaternion.identity);
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
            launchExplosion(worldPosition);
            // launchFireball(worldPosition);
        }
    }
}
