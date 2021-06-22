using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpell : MonoBehaviour
{
    public GameObject nextGO;
    public float beforeImpactX = 0, beforeImpactY = 0;

    private Transform rb;
    private Vector3 target;
    void Start()
    {
        rb = transform.GetComponent<Transform>();
        target = new Vector3(transform.parent.position.x + (float)beforeImpactX, transform.parent.position.y + (float)beforeImpactY, 0);
    }

    void FixedUpdate()
    {
        if (rb != null && rb.position != target)
        {
            rb.position = Vector3.MoveTowards(rb.position, target, Time.deltaTime * 2f);
        }
        else if (rb != null && nextGO != null)
        {
            Instantiate(nextGO, target, Quaternion.identity);
            Destroy(transform.parent.gameObject);
        }
    }
}