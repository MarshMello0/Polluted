using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class City
{
    public string name;
    public GeneratedSlot[,] slots;
    public List<Road> roads = new List<Road>();

    public Vector3 position;
    //This entrances are the points at the top and bottom of the motorway
    public Vector3 firstEntrance, secondEntrance;
    public bool firstHasConnection, secondHasConnection;

    public City(string name, GeneratedSlot[,] slots, List<Road> roads, Vector3 position)
    {
        this.name = name;
        this.slots = slots;
        this.roads = roads;
        this.position = position;
        GetEntrances();
    }

    private void GetEntrances()
    {
        for (int i = 0; i < roads.Count; i++)
        {
            //There is only ever going to be one M road
            if (roads[i].roadType == RoadType.M)
            {
                Road motorway = roads[i];
                firstEntrance = motorway.points[0] + position;
                secondEntrance = motorway.points[motorway.points.Length - 1] + position;
                return;
            }
        }
    }
    
}
