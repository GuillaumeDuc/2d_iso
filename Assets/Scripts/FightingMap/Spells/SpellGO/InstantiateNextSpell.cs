using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateNextSpell : MonoBehaviour
{
    public GameObject nextSpell;
    public float delayInstantiate = 0;
    public Vector3 modifyPos = new Vector3();

    private IEnumerator instantiateDelay()
    {
        yield return new WaitForSeconds(delayInstantiate);
        GameObject go = Instantiate(nextSpell, transform.position + modifyPos, Quaternion.identity);
        // Set spell script to next gameobject
        Spell spell = gameObject.GetComponent<Spell>();
        if (spell != null)
        {
            Spell nextSpellScript = go.AddComponent<Spell>();
            nextSpellScript.setSpell(spell);
        }
    }
    void Start()
    {
        StartCoroutine(instantiateDelay());
    }
}
