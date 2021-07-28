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
        Instantiate(nextSpell, transform.position, Quaternion.identity);
    }
    void Start()
    {
        StartCoroutine(instantiateDelay());
    }
}
