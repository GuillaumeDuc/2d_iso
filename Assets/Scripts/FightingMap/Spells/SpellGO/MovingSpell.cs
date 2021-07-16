using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpell : MonoBehaviour
{
    public GameObject movingGO;
    public GameObject nextGO;
    public float speed = 2f;

    public float beforeImpactX = 0, beforeImpactY = 0;

    public float destroyDelay = 0;

    private Transform rb;
    private Vector3 target;
    private bool reached = false;
    void Start()
    {
        rb = movingGO.GetComponent<Transform>();
        target = new Vector3(transform.position.x + (float)beforeImpactX, transform.position.y + (float)beforeImpactY, 0);
    }

    private IEnumerator delayDestroy()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(this.gameObject);
    }

    private void instantiateNextSpell()
    {
        // Instantiate
        GameObject next = Instantiate(nextGO, target, Quaternion.identity);
        // Set spell script to next gameobject
        Spell spell = gameObject.GetComponent<Spell>();
        Spell nextSpell = next.AddComponent<Spell>();
        nextSpell.setSpell(spell);
    }

    void FixedUpdate()
    {
        // Moving spell move the gameobject its attached to toward its parent
        if (rb != null && rb.position != target)
        {
            rb.position = Vector3.MoveTowards(rb.position, target, Time.deltaTime * speed);
        }
        else if (rb != null && nextGO != null && reached == false)
        {
            instantiateNextSpell();
            StartCoroutine(delayDestroy());
            reached = true;
        }
    }
}
