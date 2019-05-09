using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private bool createDebugLines;
    [SerializeField] private GameObject roadDebugLine;
    [SerializeField] private bool createDebugSquares;
    [SerializeField] private bool createRoads;
    [SerializeField] private GameObject roadTypeSquare;
    private List<RoadThread> threads = new List<RoadThread>();

    private Random rnd;
    
    public static int cityScale = 23;
    private int minAmount = 0;
    [Header("M Prefabs")] [SerializeField] private GameObject[] mRoadsPrefabs;
    [Header("A Prefabs")] [SerializeField] private GameObject[] aRoadsPrefabs;
    [Header("S Prefabs")] [SerializeField] private GameObject[] sRoadsPrefabs;
    [Header("Buildings")] [SerializeField] private GameObject[] buildingPrefabs;
    [Header("Factories")] [SerializeField] private GameObject[] factoriesPrefabs;

    [Header("City")]
    //Cities
    public City city;

    [SerializeField] private GameObject player, miniMap;
    [SerializeField] private List<Road> connectingRoads = new List<Road>();

    private Loading loading;
    private bool isLoading = true;
    private bool areConnected;
    private bool meshFlattened;
    private void Awake()
    {
        loading = FindObjectOfType<Loading>();
        loading.totalNumberOfActions = RoadThread.citySize * RoadThread.citySize;
        rnd = new Random(FindObjectOfType<GameInfo>().seed);
        
        //We are now just generating one city at the start, not endless amounts
        GenerateNewCity(Vector3.zero);
    }


    private void GenerateNewCity(Vector3 position)
    {
        RoadThread roadThread;
        roadThread = new RoadThread();
        roadThread.log = Log;
        roadThread.position = position;
        roadThread.seed = rnd.Next(Int32.MaxValue);
        threads.Add(roadThread);
        roadThread.Start();
        StartCoroutine(WaitForThread());
    }

    private IEnumerator WaitForThread()
    {
        RoadThread roadThread = threads[threads.Count - 1];
        //This will wait for that thread to be complete
        yield return StartCoroutine(roadThread.WaitFor());
        string cityName = Cities.GetRandomName();
        
        GameObject parent = new GameObject(cityName);
        parent.tag = "City";
        parent.transform.position = roadThread.position;

        float scaleBackX = roadThread.position.x * (cityScale - 1);
        float scaleBackZ = roadThread.position.z * (cityScale - 1);
        
        GeneratedSlot[,] generatedSlots = roadThread.citySlots;
        City lastCity = new City(cityName, roadThread.citySlots, roadThread.roads, roadThread.position);
        if (createDebugLines)
        {
            GameObject lineParent = new GameObject("Debug Lines");
            lineParent.transform.SetParent(parent.transform);
            for (int i = 0; i < roadThread.roads.Count; i++)
            {
                GameObject last = Instantiate(roadDebugLine, lineParent.transform);
                last.name = string.Format("{0}{1} Road", roadThread.roads[i].roadType.ToString(), i);
                LineRenderer lineRenderer = last.GetComponent<LineRenderer>();
                lineRenderer.positionCount = roadThread.roads[i].points.Length;

                for (int j = 0; j < lineRenderer.positionCount; j++)
                {
                    //This adds the offset for each point
                    Vector3 position = new Vector3(
                        ((roadThread.roads[i].points[j].x + roadThread.position.x) * cityScale) - scaleBackX,
                        ((roadThread.roads[i].points[j].y + roadThread.position.y) * cityScale),
                        ((roadThread.roads[i].points[j].z + roadThread.position.z) * cityScale) - scaleBackZ);
                    lineRenderer.SetPosition(j, position);
                }

                if (roadThread.roads[i].roadType == RoadType.M)
                {
                    lineRenderer.startColor = Color.blue;
                    lineRenderer.endColor = Color.blue;
                }
                else if (roadThread.roads[i].roadType == RoadType.A)
                {
                    lineRenderer.startColor = Color.green;
                    lineRenderer.endColor = Color.green;
                }
            }
        }
        if (createDebugSquares)
        {
            GameObject squareParent = new GameObject("Debug Squares");
            squareParent.transform.SetParent(parent.transform);
            for (int x = 0; x < generatedSlots.GetLength(0); x++)
            {
                for (int y = 0; y < generatedSlots.GetLength(1); y++)
                {
                    Vector3 position = new Vector3(
                        ((x + roadThread.position.x) * cityScale) - scaleBackX,
                        (1 + roadThread.position.y) * cityScale + 2,
                        ((y + roadThread.position.z) * cityScale) - scaleBackZ);
                    //Spawning a debug square
                    GameObject square = Instantiate(roadTypeSquare, position, Quaternion.Euler(0,0,0), squareParent.transform);
                    Material m = square.GetComponent<MeshRenderer>().material;
                    if (generatedSlots[x, y] is RoadSlot)
                    {
                        //checking what type of road it is
                        RoadSlot slot = (RoadSlot) generatedSlots[x, y];
                        if (slot.type == RoadSlot.Type.S)
                        {
                            m.SetColor("_BaseColor", Color.white);
                            //if there is no connections
                            if (!slot.hasConnection)
                            {
                                //GameObject sroad = Instantiate(sRoadsPrefabs[0],position, Quaternion.Euler(0,slot.primaryDirection,0), parent.transform);   
                            }
                        }
                        else if (slot.type == RoadSlot.Type.A)
                        {
                            m.SetColor("_BaseColor", Color.green);
                        }
                        else
                        {
                            m.SetColor("_BaseColor", Color.blue);
                        }
                    }
                    else if (generatedSlots[x,y] is BuildingSlot)
                    {
                        m.SetColor("_BaseColor", Color.magenta);
                        //Debug.Log(generatedSlots[x,y].GetType() + " X " + x + " Y " + y);
                    }
                    else if (generatedSlots[x,y] is FactorySlot)
                    {
                        if (((FactorySlot) generatedSlots[x, y]).isSpawn)
                        {
                            m.SetColor("_BaseColor", Color.yellow);
                        }
                        else
                        {
                            m.SetColor("_BaseColor", Color.red);
                        }
                        
                    }
                    else
                    {
                        m.SetColor("_BaseColor", Color.black);
                    }
                }
            }
        }
        if (createRoads)
        {
            GameObject roadsParent = new GameObject("Roads Prefabs");
            roadsParent.transform.SetParent(parent.transform);
            for (int x = 0; x < generatedSlots.GetLength(0); x++)
            {
                for (int y = 0; y < generatedSlots.GetLength(1); y++)
                {
                    Vector3 position = new Vector3(
                        ((x + roadThread.position.x) * cityScale) - scaleBackX,
                        0,
                        ((y + roadThread.position.z) * cityScale) - scaleBackZ);
                    if (generatedSlots[x, y] is RoadSlot)
                    {
                        //checking what type of road it is
                        RoadSlot slot = (RoadSlot) generatedSlots[x, y];
                        if (slot.type == RoadSlot.Type.S)
                        {
                            //if there is no connections
                            if (!slot.hasConnection)
                            {
                                GameObject sroad = Instantiate(sRoadsPrefabs[0], position,
                                    Quaternion.Euler(0, slot.primaryDirection, 0), roadsParent.transform);
                            }
                            else if (slot.connectedType.Count == 1)
                            {
                                if (slot.connectedType[0] == RoadSlot.Type.M)
                                {
                                    
                                }
                                else if (slot.connectedType[0] == RoadSlot.Type.S)
                                {
                                    GameObject sRoad = Instantiate(sRoadsPrefabs[1], position,
                                        Quaternion.Euler(0, slot.primaryDirection, 0), roadsParent.transform);
                                }
                            }
                            else if (slot.connectedType.Count == 2)
                            {
                                //This should be the max possiable connections for a road,
                                //but still doing an else if just in case not
                                if (slot.connectedType[0] == RoadSlot.Type.S &&
                                    slot.connectedType[1] == RoadSlot.Type.S)
                                {
                                    GameObject sRoad = Instantiate(sRoadsPrefabs[2], position,
                                        Quaternion.Euler(0, slot.primaryDirection, 0), roadsParent.transform);
                                }
                            }
                        }
                        else if (slot.type == RoadSlot.Type.A)
                        {
                            if (!slot.hasConnection)
                            {
                                GameObject aRoad = Instantiate(aRoadsPrefabs[0], position,
                                    Quaternion.Euler(0, slot.primaryDirection, 0), roadsParent.transform);
                            }
                            else if (slot.connectedType.Count == 1)
                            {
                                position += new Vector3(0,0.001f,0);
                                //This means there should only be 1 other road connecting to it
                                if (slot.connectedType[0] == RoadSlot.Type.M)
                                {
                                    
                                }
                                else
                                {
                                    //It must be a small road as A roads don't need to connect to each other
                                    GameObject aRoad = Instantiate(aRoadsPrefabs[1], position,
                                        Quaternion.Euler(0, slot.primaryDirection, 0), roadsParent.transform);
                                }
                            }
                            else if (slot.connectedType.Count == 2)
                            {
                                position += new Vector3(0,0.002f,0);
                                if (slot.connectedType[0] == RoadSlot.Type.S &&
                                    slot.connectedType[1] == RoadSlot.Type.S)
                                {
                                    GameObject aRoad = Instantiate(aRoadsPrefabs[2], position,
                                        Quaternion.Euler(0, slot.primaryDirection, 0), roadsParent.transform);
                                }
                            }
                        }
                        else if (slot.type == RoadSlot.Type.M)
                        {
                            if (!slot.hasConnection)
                            {
                                GameObject mRoad = Instantiate(mRoadsPrefabs[0], position,
                                    Quaternion.Euler(0, slot.primaryDirection, 0), roadsParent.transform);
                            }
                            else if (slot.connectedType.Count == 2)
                            {
                                position += new Vector3(0,0.003f,0);
                                GameObject mRoad = Instantiate(mRoadsPrefabs[1], position,
                                    Quaternion.Euler(0, slot.primaryDirection, 0), roadsParent.transform);
                            }
                        }
                    }
                    else if (generatedSlots[x, y] is BuildingSlot)
                    {
                        int index = rnd.Next(buildingPrefabs.Length);
                        
                        GameObject emptySlot = Instantiate(buildingPrefabs[index], position,
                            Quaternion.Euler(0, 0, 0), roadsParent.transform);
                    }
                    else if (generatedSlots[x,y] is EmptySlot)
                    {
                        GameObject emptySlot = Instantiate(buildingPrefabs[0], position,
                            Quaternion.Euler(0, 0, 0), roadsParent.transform);
                    }
                    else if (generatedSlots[x,y] is FactorySlot)
                    {
                        FactorySlot slot = (FactorySlot) generatedSlots[x, y];
                        
                        if (slot.isSpawn)
                        {
                            int index = rnd.Next(factoriesPrefabs.Length);
                            if (slot.rightSide)
                            {
                                GameObject factorySlot = Instantiate(factoriesPrefabs[index], position,
                                    Quaternion.Euler(0, 0, 0), roadsParent.transform);
                            }
                            else
                            {
                                //Need to move the asset over on the left side
                                GameObject factorySlot = Instantiate(factoriesPrefabs[index], position,
                                    Quaternion.Euler(0, 0, 0), roadsParent.transform);
                                factorySlot.transform.localPosition += Vector3.left * 46.88f;
                            }
                        }
                    }

                    loading.numberOfActionsCompleted++;
                }
            }
        }
        
        city = lastCity;
        
        //This calls to the RoomSwitcher to spawn the rooms in as all of the roads and buildings have been spawned
        //roomSwitcher.SetRooms();
    }

    public void Log(string message)
    {
        Debug.Log(message);
    }

    private void Update()
    {
        if (isLoading)
        {
            if (loading.totalNumberOfActions == loading.numberOfActionsCompleted)
            {
                player.SetActive(true);
                miniMap.SetActive(true);
                SceneManager.UnloadSceneAsync(1);
                isLoading = false;
            }
        }
    }
}