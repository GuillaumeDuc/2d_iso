using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMap : MonoBehaviour
{
    public CreatePathsSelection CreatePathsSelection;
    public PlacePointsSelection PlacePointsSelection;

    private LocationPoint currentLocation;
    private GameObject currentLocationGO;
    private List<LocationPoint> list;

    public void Start()
    {
        int radius = 4, rejection = 30, width = 20, height = 10;
        // Height & width match camera
        list = PlacePointsSelection.generatePoint(radius, rejection, width, height);
        CreatePathsSelection.createPath(list, radius);
        RandomLocationSelection.randomize(list, width, height);

        // Init click
        list.ForEach(a => { a.onClickAction = onClickLocation; });

        // Init selectable locations
        currentLocation = list[0];
        currentLocation.visited = true;
        // Can go to next & previous location
        currentLocation.nextLocations.ForEach(a =>
        {
            a.clickable = true;
        });
        currentLocation.previousLocations.ForEach(a =>
        {
            a.clickable = true;
        });

        // Indicate current location
        currentLocationGO = Resources.Load<GameObject>("SelectionMap/CurrentPosition/CurrentPosition");
        currentLocationGO = Instantiate(currentLocationGO, currentLocation.gameObject.transform.position, Quaternion.identity);
    }

    public void onClickLocation(LocationPoint lp)
    {
        if (lp.clickable)
        {
            lp.setVisited(true);
            // Clickable set to false, new clickable location
            currentLocation.nextLocations.ForEach(a =>
            {
                a.clickable = false;
            });
            currentLocation.previousLocations.ForEach(a =>
            {
                a.clickable = false;
            });

            lp.nextLocations.ForEach(a =>
            {
                a.clickable = true;
            });
            lp.previousLocations.ForEach(a =>
            {
                a.clickable = true;
            });

            currentLocation = lp;
            // Move location GO
            currentLocationGO.transform.position = lp.gameObject.transform.position;
        }
    }

    public void Update()
    {
    }
}
