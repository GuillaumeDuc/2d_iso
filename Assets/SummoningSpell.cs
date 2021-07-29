using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummoningSpell : MonoBehaviour
{
    public GameObject summonedUnit;

    public float delayInstantiate = 0;

    private IEnumerator instantiateDelay()
    {
        yield return new WaitForSeconds(delayInstantiate);
        GameObject go = Instantiate(summonedUnit, transform.position, Quaternion.identity);

        // Set unique name if already exists
        foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            if (gameObj.name == go.name)
            {
                go.name += System.DateTime.Now;
            }
        }

        // Add summoned unit in list
        Spell spell = gameObject.GetComponent<Spell>();
        Dictionary<Unit, GameObject> team = spell.caster.getTeam();
        Unit unit = go.GetComponent<Unit>();
        unit.summon = true;
        team.Add(unit, go);
    }
    void Start()
    {
        StartCoroutine(instantiateDelay());
    }
}
