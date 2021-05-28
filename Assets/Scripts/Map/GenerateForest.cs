using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateForest : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject Tree;

    // Start is called before the first frame update
    void Start()
    {
        Vector3Int pos = new Vector3Int(5,5,0);
        Vector3 posWorld = tilemap.CellToWorld(pos);
        posWorld.y += 0.2f;
        // Instantiate(Tree, posWorld, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
