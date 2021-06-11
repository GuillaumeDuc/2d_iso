using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateTunnels : MonoBehaviour
{
    public Tilemap tilemap;
    public TileList TileList;
    public int width, height, maxTunnels, maxLength;

    private int currentWidth, currentHeight, currentMaxTunnels, currentMaxLength;

    private void createRiver()
    {
        int currentCol = UnityEngine.Random.Range(1, width);
        int currentRow = UnityEngine.Random.Range(0, height);

        List<Vector2> directions = new List<Vector2> {
            { new Vector2(-1,0) },
            { new Vector2(1,0) },
            { new Vector2(0,-1) },
            { new Vector2(0,1) }
        };

        Vector2 lastDirection = new Vector2(), randomDirection = new Vector2();

        while (maxTunnels > 0)
        {
            do
            {
                randomDirection = directions[UnityEngine.Random.Range(0, directions.Count())];
            } while (
              (randomDirection.x == -lastDirection.x &&
                randomDirection.y == -lastDirection.y) ||
              (randomDirection.x == lastDirection.x &&
                randomDirection.y == lastDirection.y)
            );
            int randomLength = UnityEngine.Random.Range(0, maxLength),
              tunnelLength = 0;
            while (tunnelLength < randomLength)
            {
                if (
                  (currentRow == 0 && randomDirection.x == -1) ||
                  (currentCol == 0 && randomDirection.y == -1) ||
                  (currentRow == height - 1 && randomDirection.x == 1) ||
                  (currentCol == width - 1 && randomDirection.y == 1)
                )
                {
                    break;
                }
                else
                {
                    setWater(currentRow, currentCol);
                    currentRow += (int)randomDirection.x;
                    currentCol += (int)randomDirection[1];
                    tunnelLength++;
                }
            }
            lastDirection = randomDirection;
            maxTunnels--;
        }
    }

    private void setWater(int x, int y)
    {
        GroundTile tile = ScriptableObject.CreateInstance<GroundTile>();
        tile.setTile(TileList.water);
        Vector3Int pos = new Vector3Int(x, y, 0);
        tilemap.SetTile(pos, tile);
    }

    // Start is called before the first frame update
    void Start()
    {
        createRiver();
        currentWidth = width;
        currentHeight = height;
        currentMaxTunnels = maxTunnels;
        currentMaxLength = maxLength;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWidth != width || currentHeight != height || currentMaxTunnels != maxTunnels || currentMaxLength != maxLength)
        {
            createRiver();
            currentWidth = width;
            currentHeight = height;
            currentMaxTunnels = maxTunnels;
            currentMaxLength = maxLength;
        }
    }
}
