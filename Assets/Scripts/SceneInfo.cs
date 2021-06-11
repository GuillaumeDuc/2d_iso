using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneInfo
{
    // Fighting map
    public static TypeMap TypeMap;
    public static int width, height;

    // Selection map
    public static List<LocationPoint> list;
    public static LocationPoint currentLocation;
}
