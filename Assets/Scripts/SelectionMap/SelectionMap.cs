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
    private bool movePoint;
    private List<LocationPoint> list = new List<LocationPoint>();

    public void Start()
    {
        int radius = 4;
        if (SceneStore.list == null)
        {
            int rejection = 30;
            int height = 10;
            int width = 20;

            bool isValid = false;
            // Re init when ending is not possible
            do
            {
                // Remove GO & clear list
                foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
                {
                    if (o.name == "rectangle" || o.name == "LocationPoint(Clone)" || o.name == "CurrentPosition(Clone)")
                    {
                        Destroy(o);
                    }
                }
                list.Clear();

                // Height & width match camera
                list = PlacePointsSelection.generatePoint(radius, rejection, width, height);
                RandomLocationSelection.randomize(list, width, height);

                // Init selectable locations
                currentLocation = list[0];
                list[0].currentLocation = true;

                initMap(radius);

                List<LocationPoint> visited = new List<LocationPoint>();
                dfs(currentLocation, visited);
                isValid = visited.Contains(list.Find(a => { return a.TypeLocation == TypeLocation.End; }));

            } while (!isValid);
        }
        else
        {
            // Search saved list and re instantiate location points
            GameObject lpGO = Resources.Load<GameObject>("SelectionMap/LocationPoint");
            List<LocationPoint> newList = new List<LocationPoint>();
            SceneStore.list.ForEach(lp =>
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

    void dfs(LocationPoint locationPoint, List<LocationPoint> visited)
    {
        visited.Add(locationPoint);
        for (int i = 0; i < locationPoint.nextLocations.Count; i++)
        {
            if (!visited.Contains(locationPoint.nextLocations[i]))
            {
                dfs(locationPoint.nextLocations[i], visited);
            }
        }
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
        SceneStore.TypeMap = currentLocation.TypeMap;
        SceneStore.width = 50;
        SceneStore.height = 50;
    }

    public void onClickLoadScene()
    {
        SceneStore.TypeMap = currentLocation.TypeMap;

        // Save new list
        List<LocationPoint> newListLP = new List<LocationPoint>();
        list.ForEach(a =>
        {
            LocationPoint newLocationPoint = new LocationPoint();
            newLocationPoint.setLocationPoint(a);
            newListLP.Add(newLocationPoint);
        });
        SceneStore.list = newListLP;

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
        movePoint = true;

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
        if (lp.clickable && (currentLocation.cleared || lp.visited) && !movePoint)
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

    public void Update()
    {
        if (movePoint)
        {
            currentLocationGO.transform.position = Vector3.MoveTowards(currentLocationGO.transform.position, currentSelectedLocationGO.transform.position, 0.005f);
            // Stop moving
            if (currentLocationGO.transform.position == currentSelectedLocationGO.transform.position)
            {
                movePoint = false;
            }
        }
    }
}
