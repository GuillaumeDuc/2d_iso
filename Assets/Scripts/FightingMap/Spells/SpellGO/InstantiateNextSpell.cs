using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateNextSpell : MonoBehaviour
{
    public GameObject nextSpell;
    public float delayInstantiate = 0;

    private IEnumerator instantiateDelay()
    {
        yield return new WaitForSeconds(delayInstantiate);
        GameObject go = Instantiate(nextSpell, transform.position, Quaternion.identity);
        // Set spell script to next gameobject
        Spell spell = gameObject.GetComponent<Spell>();
        Spell nextSpellScript = go.AddComponent<Spell>();
        nextSpellScript.setSpell(spell);
    }
    void Start()
    {
        StartCoroutine(instantiateDelay());
    }
}
