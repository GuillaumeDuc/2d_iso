using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeLocation { Start, Location, End }

public class LocationPoint : MonoBehaviour
{
    public string nameLocation;
    public TypeMap TypeMap;
    public TypeLocation TypeLocation = TypeLocation.Location;
    public bool visited = false;
    public List<LocationPoint> nextLocations = new List<LocationPoint>();
    public List<LocationPoint> previousLocations = new List<LocationPoint>();
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
            return gameObject.transform.position == u.gameObject.transform.position;
        }
    }

    public override int GetHashCode()
    {
        return gameObject.transform.position.GetHashCode();
    }

}
