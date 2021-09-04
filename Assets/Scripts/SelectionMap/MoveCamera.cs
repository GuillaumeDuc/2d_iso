using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveCamera : MonoBehaviour
{
    private float step;
    private Vector3 dragOrigin;
    void Start()
    {
        step = 1f * Time.deltaTime;
    }

    void Update()
    {
        // Move camera with click and drag
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0) && !IsPointerOverElement())
        {
            Vector3 dif = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.position = dragOrigin - dif;
        }

        // Move camera with keys
        if (Input.GetKey("up"))
        {
            Camera.main.transform.Translate(0, step, 0, Camera.main.transform);
        }
        if (Input.GetKey("down"))
        {
            Camera.main.transform.Translate(0, -step, 0, Camera.main.transform);
        }
        if (Input.GetKey("left"))
        {
            Camera.main.transform.Translate(-step, 0, 0, Camera.main.transform);
        }
        if (Input.GetKey("right"))
        {
            Camera.main.transform.Translate(step, 0, 0, Camera.main.transform);
        }
    }

    public static bool IsPointerOverElement()
    {
        return IsPointerOverElement(GetEventSystemRaycastResults());
    }
    public static bool IsPointerOverElement(List<RaycastResult> eventSystemRaycastResults)
    {
        return eventSystemRaycastResults.Count != 0;
    }
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }
}
