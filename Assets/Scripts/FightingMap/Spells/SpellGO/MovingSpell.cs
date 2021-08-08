using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpell : MonoBehaviour
{
    public GameObject movingGO;
    public GameObject movingParticles;
    public GameObject nextGO;
    public float speed = 2f;

    public float beforeImpactX = 0, beforeImpactY = 0;

    public float destroyDelay = 0;

    private Transform rb;
    private Transform rbParticles;
    private List<ParticleSystem> ListParticleSystem = new List<ParticleSystem>();
    private Vector3 target;
    private bool reached = false;
    void Start()
    {
        // Rigid body of moving object
        if (movingGO != null)
        {
            rb = movingGO.GetComponent<Transform>();
        }
        else
        {
            rb = movingParticles.GetComponent<Transform>();
        }

        // Particles go
        if (movingParticles != null)
        {
            rbParticles = movingParticles.GetComponent<Transform>();
            // Set position
            float x = rbParticles.position.x - transform.position.x;
            float y = rbParticles.position.y - transform.position.y;
            rbParticles.position = new Vector3(rb.position.x, rb.position.y, rb.position.z);
            // Set rotation
            rbParticles.rotation = rb.rotation;
            // GameObject is included in list
            List<Transform> listChild = rbParticles.GetComponentsInChildren<Transform>().ToList();
            listChild.RemoveAt(0);
            listChild.ForEach(go =>
            {
                ListParticleSystem.Add(go.GetComponent<ParticleSystem>());
            });
        }

        // Set target
        target = new Vector3(transform.position.x + (float)beforeImpactX, transform.position.y + (float)beforeImpactY, 0);
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
        // Move movingGO & Particles toward current GameObject
        if (rb != null && rb.position != target)
        {
            rb.position = Vector3.MoveTowards(rb.position, target, Time.deltaTime * speed);
            if (rbParticles != null)
            {
                rbParticles.position = Vector3.MoveTowards(rbParticles.position, target, Time.deltaTime * speed);
            }
        }
        else if (rb != null && !reached)
        {
            if (nextGO != null)
            {
                instantiateNextSpell();
            }
            // Destroy moving go
            Destroy(movingGO);
            // Destroy current go with delay
            Destroy(this.gameObject, destroyDelay);
            reached = true;
        }

        // Stop particles
        if (reached && ListParticleSystem.Count != 0)
        {
            ListParticleSystem.ForEach(p =>
            {
                var main = p.main;
                main.loop = false;
            });
        }
    }
}
