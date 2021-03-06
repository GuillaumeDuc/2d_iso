using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TypeMap { Beach, Desert, Forest }
public class CreateMap : MonoBehaviour
{
    public TileList TileList;
    public Tilemap tilemap;

    private TypeMap TypeMap;
    private int width, height;

    // Start is called before the first frame update
    void Start()
    {
        // Get map dimensions
        if (SceneStore.width == 0)
        {
            TypeMap = TypeMap.Forest;
            width = 25;
            height = 25;
        }
        else
        {
            TypeMap = SceneStore.TypeMap;
            width = SceneStore.width;
            height = SceneStore.height;
        }

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        GameObject Tree = Resources.Load<GameObject>("Obstacles/Trees/Tree");
        switch (TypeMap)
        {
            case TypeMap.Beach:
                int waterHeightMin = height / 6, waterHeightMax = height / 4;
                GenerateBeach.createBeach(width, height, waterHeightMin, waterHeightMax, TileList, tilemap);
                break;
            case TypeMap.Desert:
                GenerateGround.fillMap(width, height, TileList.sand, tilemap);
                break;
            case TypeMap.Forest:
            default:
                int spawnsWater = height / 10,
                    spawnsTree = height / 6,
                    radius = 2,
                    rejection = 5,
                    widthWater = height / 3;
                GenerateForest.createForest(
                    spawnsTree,
                    radius,
                    rejection,
                    widthWater,
                    spawnsWater,
                    width,
                    height,
                    TileList,
                    tilemap,
                    Tree
                );
                break;
        }
        stopwatch.Stop();
        Debug.Log("Elapsed Time is " + stopwatch.ElapsedMilliseconds + " ms");
    }
}
