using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneStore
{
    // Fighting map
    public static TypeMap TypeMap;
    public static int width, height;

    // Selection map
    public static List<LocationPoint> list;
    public static LocationPoint currentLocation;

    // Selection spell
    public static List<GameObject> selectedSpellList;
}
