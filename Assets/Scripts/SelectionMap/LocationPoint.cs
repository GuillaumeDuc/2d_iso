using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TypeLocation { Start, Location, End }

public class LocationPoint : MonoBehaviour, IPointerClickHandler
{
    public string nameLocation;
    public TypeMap TypeMap;
    public TypeLocation TypeLocation = TypeLocation.Location;
    public bool visited = false, clickable = false, cleared = false, currentLocation = false;
    public List<LocationPoint> nextLocations = new List<LocationPoint>();
    public List<LocationPoint> previousLocations = new List<LocationPoint>();
    public System.Action<LocationPoint> onClickAction;
    public Vector2 position;

    public void setLocationPoint(LocationPoint lp)
    {
        nameLocation = lp.nameLocation;
        TypeMap = lp.TypeMap;
        TypeLocation = lp.TypeLocation;
        visited = lp.visited;
        clickable = lp.clickable;
        cleared = lp.cleared;
        position = lp.position;
        currentLocation = lp.currentLocation;
    }

    public void setNPClickable(bool clickable)
    {
        nextLocations.ForEach(a =>
                {
                    a.clickable = clickable;
                });
        previousLocations.ForEach(a =>
        {
            a.clickable = clickable;
        });
    }

    public void setIcon(TypeMap TypeMap)
    {
        this.TypeMap = TypeMap;
        Sprite sprite = null;
        if (TypeLocation == TypeLocation.Start)
        {
            sprite = Resources.Load<Sprite>("SelectionMap/start_icon");
            nameLocation = "Start";
        }
        else if (TypeLocation == TypeLocation.End)
        {
            sprite = Resources.Load<Sprite>("SelectionMap/end_icon");
            nameLocation = "End";
        }
        else
        {
            switch (TypeMap)
            {
                case TypeMap.Beach:
                    sprite = Resources.Load<Sprite>("SelectionMap/beach_icon");
                    nameLocation = "Beach";
                    break;
                case TypeMap.Forest:
                    sprite = Resources.Load<Sprite>("SelectionMap/forest_icon");
                    nameLocation = "Forest";
                    break;
                case TypeMap.Desert:
                    sprite = Resources.Load<Sprite>("SelectionMap/desert_icon");
                    nameLocation = "Desert";
                    break;
            }
        }
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void setVisited(bool visited)
    {
        this.visited = visited;
        setColor();
    }

    public void setCleared(bool cleared)
    {
        this.cleared = cleared;
        setColor();
    }

    public void setColor()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (cleared)
        {
            sr.color = Color.black;
        }
        else if (visited)
        {
            sr.color = Color.gray;
        }
        else
        {
            sr.color = Color.white;
        }
    }

    public override bool Equals(System.Object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            LocationPoint u = (LocationPoint)obj;
            return position == u.position;
        }
    }

    public override int GetHashCode()
    {
        return gameObject.transform.position.GetHashCode();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickAction?.Invoke(this);
    }
}
