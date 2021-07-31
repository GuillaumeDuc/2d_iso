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
        foreach (var a in FightingSceneStore.initiativeList)
        {
            if (a.Key.name == go.name)
            {
                go.name += System.Guid.NewGuid();
            }
        };

        // Get Spell
        Spell spell = gameObject.GetComponent<Spell>();
        // Get caster's team
        Dictionary<Unit, GameObject> team = spell.caster.getTeam();
        // Set summoned unit to summoned
        Unit unit = go.GetComponent<Unit>();
        unit.summon = true;
        // Add to caster's team
        team.Add(unit, go);
        // Add to ScrollView
    }
    void Start()
    {
        StartCoroutine(instantiateDelay());
    }
}
