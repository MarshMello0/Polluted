using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class RoadThread : ThreadedJob
{
    //This log action is so I can do debug.log on the main thread
    public Action<string> log;
    
    public GeneratedSlot[,] citySlots;
    public List<Road> roads;
    public int citySize = 100;

 
    // Do your threaded task. DON'T use the Unity API here
    protected override void ThreadFunction()
    {
        /*
         * I could of split each type of generation into their own methods, but I just used regions instead :/
         */
        //The C# Random instance
        Random rnd = new Random();
        
        
        //Set the size of the whole city
        citySlots = new GeneratedSlot[citySize,citySize];
        //Setting the list
        roads = new List<Road>();
        
        //Setting all the tiles first so we don't get any errors later on
        EmptySlot emptySlot = new EmptySlot();
        for (int x = 0; x < citySize; x++)
        {
            for (int y = 0; y < citySize; y++)
            {
                citySlots[x,y] = emptySlot;
            }
        }

        #region Motorway

        /*
         * 
         * Motorway
         * 
         */
        
        //Generate the one motorway from top to bottom
        int motorwayStartX = rnd.Next(citySize); //Picks a random number from 0 to city size
        
        //The motorway is always going to be the size of the city
        Vector3[] points = new Vector3[citySize];
        
        //The first slot won't be connected to anything
        citySlots[motorwayStartX,0] = new RoadSlot();
        points[0] = new Vector3(motorwayStartX,0,0);
        //This for loop will go from top to bottom along the one x axis to create the motorway
        for (int i = 1; i < citySize; i++)
        {
            citySlots[motorwayStartX,i] = new RoadSlot(motorwayStartX,i - 1);
            points[i] = new Vector3(motorwayStartX,0, i);
        }
        
        Road motorway = new Road(points, RoadType.M);
        
        roads.Add(motorway);
        #endregion

        #region A-Road

        /*
         *
         * A-Road
         *
         * Each side of the motorway there will be 3 different A roads unless
         * the motorway is less that the minium size for small houses 
         */
        
        //Left side
        Vector3[] a1Points = new Vector3[motorwayStartX];
        Vector3[] a2Points = new Vector3[motorwayStartX];
        Vector3[] a3Points = new Vector3[motorwayStartX];
        
        //Right side
        Vector3[] a4Points = new Vector3[citySize - motorwayStartX];
        Vector3[] a5Points = new Vector3[citySize - motorwayStartX];
        Vector3[] a6Points = new Vector3[citySize - motorwayStartX];
        
        
        
        for (int i = 0; i < motorwayStartX; i++)
        {
            CheckSlotForRoad(motorwayStartX - i,0,(motorwayStartX - i) + 1,0);
            a1Points[i] = new Vector3(motorwayStartX - i,0,0);
            
            CheckSlotForRoad(motorwayStartX - i,citySize / 2,(motorwayStartX - i) + 1,citySize / 2);
            a2Points[i] = new Vector3(motorwayStartX - i,0,citySize / 2);
           
            CheckSlotForRoad(motorwayStartX - i,citySize - 1,(motorwayStartX - i) + 1,citySize - 1);
            a3Points[i] = new Vector3(motorwayStartX - i,0,citySize - 1);
        }

        for (int i = 0; i < citySize - motorwayStartX; i++)
        {
            CheckSlotForRoad(motorwayStartX + i,0,(motorwayStartX + i) - 1,0);
            a4Points[i] = new Vector3(motorwayStartX + i,0,0);
            
            CheckSlotForRoad(motorwayStartX + i,citySize / 2,(motorwayStartX + i) - 1,citySize / 2);
            a5Points[i] = new Vector3(motorwayStartX + i,0,citySize / 2);
            
            CheckSlotForRoad(motorwayStartX + i,citySize - 1,(motorwayStartX + i) - 1,citySize - 1);
            a6Points[i] = new Vector3(motorwayStartX + i,0,citySize - 1);
        }
        
        Road a1 = new Road(a1Points, RoadType.A);
        Road a2 = new Road(a2Points, RoadType.A);
        Road a3 = new Road(a3Points, RoadType.A);
        Road a4 = new Road(a4Points, RoadType.A);
        Road a5 = new Road(a5Points, RoadType.A);
        Road a6 = new Road(a6Points, RoadType.A);
        
        roads.Add(a1);
        roads.Add(a2);
        roads.Add(a3);
        roads.Add(a4);
        roads.Add(a5);
        roads.Add(a6);

        #endregion

        #region S-Road Housing

        /*
         *
         * Side Road Housing
         *
         * 
         */

        //These two roads are the roads we are going to be finding the random points off of
        Road leftStraightRoad = a1;
        Road rightStraightRoad = a4;
        int amountOfSideRoads = 10; //This number will be used to how many random points we find
        
        

        for (int i = 0; i < (citySize / 2) - 1; i++)
        {
            Vector3[] leftRoadPoints = new Vector3[motorwayStartX - 1];
            Vector3[] rightRoadPoints = new Vector3[citySize - motorwayStartX - 1];
            
            Vector2[] leftRandomPoints = new Vector2[amountOfSideRoads];
            Vector2[] rightRandomPoints = new Vector2[amountOfSideRoads];
            
            //First find the random points that we want to use
            for (int j = 0; j < amountOfSideRoads; j++)
            {
                //This will give a random point of the points list to pick from
                int leftPoint = rnd.Next(leftStraightRoad.points.Length);
                int rightPoint = rnd.Next(rightStraightRoad.points.Length);

                Vector2 leftVector2 = leftStraightRoad.points[leftPoint];
                Vector2 rightVector2 = rightStraightRoad.points[rightPoint];

                leftRandomPoints[j] = leftVector2;
                rightRandomPoints[j] = rightVector2;
            }
            
            
            //Loop for the whole length that the road needs to be
            for (int j = 0; j < citySize - motorwayStartX - 1; j++)
            {
                int x = motorwayStartX + j + 1;
                int y = i + 1;
                
                RoadSlot rightSlot = new RoadSlot();
                rightRoadPoints[j] = new Vector3(x,0,y);
                for (int k = 0; k < rightRandomPoints.Length; k++)
                {
                    if (rightRandomPoints[k].x == motorwayStartX + j + 1)
                    {
                        rightSlot.AddConnection(x, i);
                        
                        Vector3[] smallRoadPoints = new Vector3[2];
                        smallRoadPoints[0] = new Vector3(x,0,y);
                        smallRoadPoints[1] = new Vector3(x,0,i);
                        roads.Add(new Road(smallRoadPoints, RoadType.S));
                        
                        break;
                    }
                }

                citySlots[x, y] = rightSlot;
            }

            for (int j = 0; j < motorwayStartX - 1; j++)
            {
                int x = motorwayStartX - j - 1;
                int y = i + 1;
                
                RoadSlot leftSlot = new RoadSlot();
                leftRoadPoints[j] = new Vector3(x,0,y);
                for (int k = 0; k < leftRandomPoints.Length; k++)
                {
                    if (leftRandomPoints[k].x == x)
                    {
                        leftSlot.AddConnection(x,i);
                        
                        Vector3[] smallRoadPoints = new Vector3[2];
                        smallRoadPoints[0] = new Vector3(x,0,y);
                        smallRoadPoints[1] = new Vector3(x,0,i);
                        roads.Add(new Road(smallRoadPoints, RoadType.S));
                        
                        break;
                    }
                }

                citySlots[x, y] = leftSlot;
            }

            Road left = new Road(leftRoadPoints, RoadType.S);
            Road right = new Road(rightRoadPoints, RoadType.S);

            leftStraightRoad = left;
            rightStraightRoad = right;
            
            roads.Add(left);
            roads.Add(right);
        }
        
        //Connecting the the middle A roads
        
        Vector2[] leftLastRandomPoints = new Vector2[amountOfSideRoads];
        Vector2[] rightLastRandomPoints = new Vector2[amountOfSideRoads];
        
        for (int i = 0; i < amountOfSideRoads; i++)
        {
            Vector3[] leftPoints = new Vector3[2];
            Vector3[] rightPoints = new Vector3[2];
            
            //This will give a random point of the points list to pick from
            int leftPoint = rnd.Next(leftStraightRoad.points.Length);
            int rightPoint = rnd.Next(rightStraightRoad.points.Length);
            
            leftPoints[0] = new Vector3(leftStraightRoad.points[leftPoint].x,0,(citySize / 2) - 1);
            leftPoints[1] = new Vector3(leftStraightRoad.points[leftPoint].x,0,(citySize / 2));
            
            rightPoints[0] = new Vector3(rightStraightRoad.points[rightPoint].x,0,(citySize / 2) - 1);
            rightPoints[1] = new Vector3(rightStraightRoad.points[rightPoint].x,0,(citySize / 2));
            
            //These float should already be a whole number anyways
            int rightX = Mathf.RoundToInt(rightPoints[0].x); 
            int leftX = Mathf.RoundToInt(leftPoints[0].x);
            int y = Mathf.RoundToInt(leftStraightRoad.points[0].z);
            
            //Adding it to the slot array
            CheckSlotForRoad(rightX,y,rightX,y + 1);
            CheckSlotForRoad(rightX,y + 1,rightX,y);
            
            CheckSlotForRoad(leftX,y,leftX,y + 1);
            CheckSlotForRoad(leftX,y + 1,leftX,y);
            
            Road leftSmallRoad = new Road(leftPoints,RoadType.S);
            Road rightSmallRoad = new Road(rightPoints, RoadType.S);
            
            roads.Add(leftSmallRoad);
            roads.Add(rightSmallRoad);
        }
        #endregion

        #region S-Road Factories

        /*
         *
         * S-Road Factories
         *
         * The way the roads are going to generate will be based of a fixed size
         * So for every 10 units, we place a road then inside of that 10 units we would spawn the prefab for the factory
         * if there is less that 10 units left we just stop
         *
         * If there is less than 10 units on the whole side we just leave that side.
         */
        
        //First we want to work out how many times we can move along on one side
        //
        float factorySize = 10;
        int leftFactoryRoads = Mathf.FloorToInt(motorwayStartX / factorySize);
        int rightFactoryRoads = Mathf.FloorToInt((citySize - motorwayStartX) / factorySize);
        
        //Left side
        for (int i = 1; i <= leftFactoryRoads; i++)
        {
            int roadX = Mathf.RoundToInt(motorwayStartX - (factorySize * i));
            
            //For the distance between the middle road and the top road, we create a straight line on x
            Vector3[] currentRoadPoints = new Vector3[citySize / 2]; //This should be half the size
            for (int j = 0; j < citySize / 2; j++)
            {
                int y = j + citySize / 2;
                currentRoadPoints[j] = new Vector3(roadX,0,y);

                if (j == 0)
                {
                    //There should be a A-road in this slot, so we are going to connect one up
                    CheckSlotForRoad(roadX,y,roadX,y + 1);
                }
                else
                {
                    //From now on we connect one down
                    CheckSlotForRoad(roadX,y,roadX,y - 1);
                }
            }
            
            Road currentRoad = new Road(currentRoadPoints, RoadType.S);
            roads.Add(currentRoad);
        }
        
        //Right Side
        for (int i = 1; i <= rightFactoryRoads; i++)
        {
            int roadX = Mathf.RoundToInt(motorwayStartX + (factorySize * i));
            
            //For the distance between the middle road and the top road, we create a straight line on x
            Vector3[] currentRoadPoints = new Vector3[citySize / 2]; //This should be half the size
            for (int j = 0; j < citySize / 2; j++)
            {
                int y = j + citySize / 2;
                currentRoadPoints[j] = new Vector3(roadX,0,y);

                if (j == 0)
                {
                    //There should be a A-road in this slot, so we are going to connect one up
                    CheckSlotForRoad(roadX,y,roadX,y + 1);
                }
                else
                {
                    //From now on we connect one down
                    CheckSlotForRoad(roadX,y,roadX,y - 1);
                }
            }
            
            Road currentRoad = new Road(currentRoadPoints, RoadType.S);
            roads.Add(currentRoad);
        }

        #endregion
    }

    private void CheckSlotForRoad(int x, int y, int connectedX, int connectedY)
    {
        if (citySlots[x,y] is RoadSlot)
        {
            ((RoadSlot)citySlots[x, y]).AddConnection(connectedX,connectedY);
        }
        else
        {
            citySlots[x,y] = new RoadSlot(connectedX,connectedY);
        }
    }
}

public class GeneratedSlot
{

}

public class BuildingSlot : GeneratedSlot
{
    
}

public class RoadSlot : GeneratedSlot
{
    public bool hasConnection;
    public List<int> connectedX = new List<int>();
    public List<int> connectedY = new List<int>();
    
    public RoadSlot()
    {
        hasConnection = false;
    }

    public RoadSlot(int connectedX, int connectedY)
    {
        hasConnection = true;
        this.connectedX.Add(connectedX);
        this.connectedY.Add(connectedY);
    }

    public void AddConnection(int connectedX, int connectedY)
    {
        this.connectedX.Add(connectedX);
        this.connectedY.Add(connectedY);
        hasConnection = true;
    }
}

public class EmptySlot : GeneratedSlot
{
    
}

public class Road
{
    public Vector3[] points;
    public RoadType roadType;

    public Road(Vector3[] points,RoadType roadType)
    {
        this.points = points;
        this.roadType = roadType;
    }
}

public enum RoadType
{
    M,A,S
}