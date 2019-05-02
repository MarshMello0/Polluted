using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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


    [Header("Settings")] 
    [SerializeField] private int cityDistance = 200;

    [SerializeField] private int spawnSize = 1000;
    
    //These are for generating new cities
    [SerializeField] private float updateDistance;
    [SerializeField] private Transform updatePoint;
    private Vector3 lastUpdatedPosition = Vector3.zero;
    
    
    public static int cityScale = 23;
    private int minAmount = 0;
    [Header("M Prefabs")] [SerializeField] private GameObject[] mRoadsPrefabs;
    [Header("A Prefabs")] [SerializeField] private GameObject[] aRoadsPrefabs;
    [Header("S Prefabs")] [SerializeField] private GameObject[] sRoadsPrefabs;
    [Header("Buildings")] [SerializeField] private GameObject[] buildingPrefabs;
    [Header("Cities")]
    //Cities
    public List<City> cities = new List<City>();
    [SerializeField] private List<Road> connectingRoads = new List<Road>();

    private Loading loading;
    private bool areConnected;
    private bool meshFlattened;
    private void Awake()
    {
        loading = FindObjectOfType<Loading>();
        rnd = new Random(FindObjectOfType<GameInfo>().seed);
        minAmount = Mathf.FloorToInt(Mathf.RoundToInt((spawnSize * 94) * 2.5f) / (cityDistance * 2f));
        minAmount = 1;
        for (int i = 0; i < minAmount; i++)
        {
            Generator(Vector3.zero);
        }
    }

    private void Update()
    {
        if (!areConnected)
        {
            ConnectCities();
        }

        if (Vector3.Distance(lastUpdatedPosition, updatePoint.position) > updateDistance)
        {
            lastUpdatedPosition = updatePoint.position;
            Generator(lastUpdatedPosition);
        }
    }

    private void Generator(Vector3 offset, int count = 0)
    {
        int x = rnd.Next(-spawnSize,spawnSize);
        int z = rnd.Next(-spawnSize,spawnSize);
        x *= 94;
        x = Mathf.RoundToInt(x * 2.5f);
        z *= 94;
        z = Mathf.RoundToInt(z * 2.5f);
        z -= 100;
        x -= 100;
        Vector3 position = new Vector3(x ,0,z);
        position += new Vector3(offset.x, 0, offset.z);

        for (int j = 0; j < cities.Count; j++)
        {
            if (Vector3.Distance(cities[j].position, position) < cityDistance)
            {
                //Too Close, Re generate
                if (count > 5)
                {
                    //This will only check 5 times
                    Generator(offset, count += 1);
                    return;
                }

                return;
            }
        }
        GenerateNewCity(position);
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
        
        GameObject parent = new GameObject(cityName, typeof(DisableGO));
        parent.transform.position = roadThread.position;

        float scaleBackX = roadThread.position.x * (cityScale - 1);
        float scaleBackZ = roadThread.position.z * (cityScale - 1);
        
        GeneratedSlot[,] generatedSlots = roadThread.citySlots;
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
                        (1 + roadThread.position.y) * cityScale + 2,
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
                        GameObject emptySlot = Instantiate(buildingPrefabs[rnd.Next(buildingPrefabs.Length)], position,
                            Quaternion.Euler(0, 0, 0), roadsParent.transform);
                    }
                    else if (generatedSlots[x,y] is EmptySlot)
                    {
                        GameObject emptySlot = Instantiate(buildingPrefabs[0], position,
                            Quaternion.Euler(0, 0, 0), roadsParent.transform);
                    }
                }
            }
        }
        
        //Adding to the cities list for help connecting roads
        cities.Add(new City(cityName,roadThread.citySlots,roadThread.roads, roadThread.position)); //this is required to flatten the terrain
    }

    public void Log(string message)
    {
        Debug.Log(message);
    }

    private void ConnectCities()
    {
        if (minAmount == cities.Count)
        {
            Debug.Log("All of the cities have been generated");
            areConnected = true;
            City firstEntranceCity = null;
            City secondEntranceCity = null;
            foreach (City city in cities)
            {
                //I added these bool because there was an error when nothing had changed as there already a connection
                bool firstHasChanged = false;
                bool secondHasChanged = false;
                //Down on Z for first point
                //Up on Z for second
                float firstDistance = float.MaxValue;
                float secondDistance = float.MaxValue;
                foreach (City checkCity in cities)
                {
                    if (city.firstHasConnection)
                    {
                        //If the bottom already has a connection
                        //Debug.Log(string.Format("City {0} already has a first connection", city.name));
                        continue;
                    }
                    else if (checkCity.secondEntrance.z < city.firstEntrance.z && !checkCity.secondHasConnection)
                    {
                        if (Vector3.Distance(checkCity.secondEntrance, city.firstEntrance) < firstDistance)
                        {
                            firstDistance = Vector3.Distance(checkCity.secondEntrance, city.firstEntrance);
                            firstEntranceCity = checkCity;
                            city.firstHasConnection = true;
                            firstHasChanged = true;
                        }
                        else continue;
                    }
                    else if (city.secondHasConnection)
                    {
                        //If the top has already been connected
                        //Debug.Log(string.Format("City {0} already has a second connection", city.name));
                        continue;
                    }
                    else if (checkCity.firstEntrance.z > city.secondEntrance.z && !checkCity.firstHasConnection)
                    {
                        if (Vector3.Distance(checkCity.firstEntrance, city.secondEntrance) < secondDistance)
                        {
                            secondDistance = Vector3.Distance(checkCity.firstEntrance, city.secondEntrance);
                            secondEntranceCity = checkCity;
                            city.secondHasConnection = true;
                            secondHasChanged = true;
                        }
                        continue;
                    }
                }

                if (firstHasChanged)
                {
                    //Debug.Log(string.Format("Connecting {0}'s bottom to {1}'s top", city.name,firstEntranceCity.name));
                    CreateConnectingRoads(city.firstEntrance, firstEntranceCity.secondEntrance);
                    firstEntranceCity.secondHasConnection = true;
                }

                if (secondHasChanged)
                {
                    //Debug.Log(string.Format("Connecting {0}'s top to {1}'s bottom", city.name,secondEntranceCity.name));
                    CreateConnectingRoads(city.secondEntrance, secondEntranceCity.firstEntrance);
                    secondEntranceCity.firstHasConnection = true;
                }
            }
        }
    }
    
    

    private void CreateConnectingRoads(Vector3 start, Vector3 end)
    {
        if (createDebugLines)
        {
            GameObject last = Instantiate(roadDebugLine);
            last.name = "Connecting Road";
            LineRenderer lineRenderer = last.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.green;
        }
    }
}