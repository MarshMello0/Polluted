using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    //Each road has a list of points of where it goes
    public List<Transform> points = new List<Transform>();
    public RoadType roadType;
    public Road(RoadType roadType)
    {
        this.roadType = roadType;
    }

    public void ColliderHit(int position)
    {
        if (position == 0)
            return;
        int positionToRemoveAt = -1;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i >= position)
            {
                if (positionToRemoveAt == -1)
                {
                    positionToRemoveAt = i + 1;
                }
                points.RemoveAt(positionToRemoveAt);
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
