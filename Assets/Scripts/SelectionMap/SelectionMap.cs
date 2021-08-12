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
    public GameObject TravelButtonUI;

    private LocationPoint currentLocation;
    private LocationPoint currentSelectedLocation;
    private GameObject currentLocationGO;
    private GameObject currentSelectedLocationGO;
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

        // Indicate selected location
        currentSelectedLocationGO = Resources.Load<GameObject>("SelectionMap/SelectedLocation/SelectedLocation");
        currentSelectedLocationGO = Instantiate(currentSelectedLocationGO, currentLocation.position, Quaternion.identity);
        currentSelectedLocationGO.SetActive(false);

        // Load Scene button
        Button LoadSceneButton = LoadSceneButtonUI.GetComponentInChildren<Button>();
        LoadSceneButton.onClick.AddListener(onClickLoadScene);

        // Set up Travel button
        Button TravelButton = TravelButtonUI.GetComponentInChildren<Button>();
        TravelButton.onClick.AddListener(onClickTravel);

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

    public void onClickTravel()
    {
        // Hide selected location GameObject
        currentSelectedLocationGO.SetActive(false);
        // Hide travel location button
        TravelButtonUI.SetActive(false);

        // Move location GO
        currentLocation.setVisited(true);
        currentLocation.currentLocation = false;
        currentSelectedLocation.currentLocation = true;

        // Clickable set to false, new clickable location
        currentLocation.setNPClickable(false);
        currentSelectedLocation.setNPClickable(true);

        currentLocation = currentSelectedLocation;
        // Move location GO
        currentLocationGO.transform.position = currentSelectedLocation.position;

        // Show or hide Load Scene Button
        if (!currentLocation.cleared)
        {
            LoadSceneButtonUI.SetActive(true);
            LoadSceneButtonUI.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "Enter " + currentLocation.nameLocation;
        }
        else
        {
            LoadSceneButtonUI.SetActive(false);
        }
    }

    public void onClickLocation(LocationPoint lp)
    {
        // Show travel button
        if (lp.clickable && (currentLocation.cleared || lp.visited))
        {
            // Move GameObject
            currentSelectedLocationGO.SetActive(true);
            currentSelectedLocationGO.transform.position = lp.position;
            // Set button
            string text = "Go to " + lp.nameLocation;
            TravelButtonUI.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = text;
            TravelButtonUI.SetActive(true);
            // Change selected location
            currentSelectedLocation = lp;
        }
    }
}
