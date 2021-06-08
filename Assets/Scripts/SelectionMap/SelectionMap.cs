using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMap : MonoBehaviour
{
    public CreatePathsSelection CreatePathsSelection;
    public PlacePointsSelection PlacePointsSelection;

    private LocationPoint currentLocation;

    public void Start()
    {
        int radius = 4, rejection = 30, width = 20, height = 10;
        // Height & width match camera
        List<LocationPoint> list = PlacePointsSelection.generatePoint(radius, rejection, width, height);
        CreatePathsSelection.createPath(list, radius);
        RandomLocationSelection.randomize(list, width, height);
    }
}
