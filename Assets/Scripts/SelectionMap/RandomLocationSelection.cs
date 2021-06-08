using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomLocationSelection
{
    public static void randomize(List<LocationPoint> list, int width, int height)
    {
        // Start point is first in list, end point is second
        LocationPoint firstPoint = list[0];
        LocationPoint endPoint = list[1];

        list.ForEach(l =>
        {
            // Special icon for end and start on map
            if (firstPoint == l)
            {
                l.TypeLocation = TypeLocation.Start;
            }
            else if (endPoint == l)
            {
                l.TypeLocation = TypeLocation.End;
            } else
            {
                l.TypeLocation = TypeLocation.Location;
            }

            // Beach
            if (l.gameObject.transform.position.y >= height - (height / 5))
            {
                
                l.setIcon(TypeMap.Beach);
            }
            // Forest & Desert
            else
            {
                int type = UnityEngine.Random.Range(1, 3);
                switch (type)
                {
                    case 1:

                        l.setIcon(TypeMap.Forest);
                        break;
                    case 2:
                        l.setIcon(TypeMap.Desert);
                        break;
                }
            }
        });
    }
}
