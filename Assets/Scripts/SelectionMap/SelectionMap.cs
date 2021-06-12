using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionMap : MonoBehaviour
{
    public CreatePathsSelection CreatePathsSelection;
    public PlacePointsSelection PlacePointsSelection;

    public GameObject LoadSceneButtonUI;

    private LocationPoint currentLocation;
    private GameObject currentLocationGO;
    private List<LocationPoint> list;

    public void Start()
    {
        int radius = 4;
        if (SceneInfo.list == null)
        {
            int rejection = 30, width = 20, height = 10;
            // Height & width match camera
            list = PlacePointsSelection.generatePoint(radius, rejection, width, height);
            RandomLocationSelection.randomize(list, width, height);

            // Init selectable locations
            currentLocation = list[0];
            list[0].currentLocation = true;

            initMap(radius);
        }
        else
        {
            // Search saved list and re instantiate location points
            GameObject lpGO = Resources.Load<GameObject>("SelectionMap/LocationPoint");
            List<LocationPoint> newList = new List<LocationPoint>();
            SceneInfo.list.ForEach(lp =>
            {
                GameObject go = Instantiate(lpGO, lp.position, Quaternion.identity);
                LocationPoint newLP = go.GetComponent<LocationPoint>();
                // Set point
                newLP.setLocationPoint(lp);
                // Set Icon
                newLP.setIcon(newLP.TypeMap);
                // Set Color
                newLP.setColor();

                newList.Add(newLP);
            });
            list = newList;

            // Get current Location
            currentLocation = list.Find(a => a.currentLocation);

            initMap(radius);
        }
        currentLocation.setCleared(true);
        currentLocation.setVisited(true);
    }

    public void initMap(int radius)
    {
        // Create Path
        CreatePathsSelection.createPath(list, radius);

        // Init click
        list.ForEach(a => { a.onClickAction = onClickLocation; });

        // Can go to next & previous location
        currentLocation.setNPClickable(true);

        // Indicate current location
        currentLocationGO = Resources.Load<GameObject>("SelectionMap/CurrentPosition/CurrentPosition");
        currentLocationGO = Instantiate(currentLocationGO, currentLocation.position, Quaternion.identity);

        // Load Scene button
        Button LoadSceneButton = LoadSceneButtonUI.GetComponentInChildren<Button>();
        LoadSceneButton.onClick.AddListener(onClickLoadScene);

        // Set parameters to scene
        SceneInfo.TypeMap = currentLocation.TypeMap;
        SceneInfo.width = 50;
        SceneInfo.height = 50;
    }

    public void onClickLoadScene()
    {
        SceneInfo.TypeMap = currentLocation.TypeMap;

        // Save new list
        List<LocationPoint> newListLP = new List<LocationPoint>();
        list.ForEach(a =>
        {
            LocationPoint newLocationPoint = new LocationPoint();
            newLocationPoint.setLocationPoint(a);
            newListLP.Add(newLocationPoint);
        });
        SceneInfo.list = newListLP;

        SceneManager.LoadScene("FightingMap");
    }

    public void onClickLocation(LocationPoint lp)
    {
        // Move
        if (lp.clickable && (currentLocation.cleared || lp.visited))
        {
            currentLocation.setVisited(true);
            currentLocation.currentLocation = false;
            lp.currentLocation = true;
            // Clickable set to false, new clickable location
            currentLocation.setNPClickable(false);
            lp.setNPClickable(true);

            currentLocation = lp;
            // Move location GO
            currentLocationGO.transform.position = lp.position;
            // Show or hide Load Scene Button
            if (!currentLocation.cleared)
            {
                LoadSceneButtonUI.SetActive(true);
                LoadSceneButtonUI.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "Go to " + currentLocation.nameLocation;
            }
            else
            {
                LoadSceneButtonUI.SetActive(false);
            }
        }
    }
}
