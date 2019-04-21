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
    private List<RoadThread> threads = new List<RoadThread>();

    private Random rnd;


    [Header("Settings")] 
    [SerializeField] private int cityDistance = 200;

    [SerializeField] private int spawnSize = 1000;
    private int minAmount = 0;
    [Header("Prefabs")] 
    [SerializeField] private GameObject terrainLoader;
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
        for (int i = 0; i < minAmount; i++)
        {
            Generator();
        }
    }

    private void Update()
    {
        if (!areConnected)
        {
            ConnectCities();
        }
    }

    private void Generator()
    {
        int x = rnd.Next(-spawnSize,spawnSize);
        int z = rnd.Next(-spawnSize,spawnSize);
        x *= 94;
        x = Mathf.RoundToInt(x * 2.5f);
        z *= 94;
        z = Mathf.RoundToInt(z * 2.5f);
        Vector3 position = new Vector3(x - 50,0,z - 50);

        for (int j = 0; j < cities.Count; j++)
        {
            if (Vector3.Distance(cities[j].position, position) < cityDistance)
            {
                //Too Close, Re generate
                Generator();
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
        if (createDebugLines)
        {
            GameObject parent = new GameObject(cityName);
            parent.transform.position = roadThread.position;
            for (int i = 0; i < roadThread.roads.Count; i++)
            {
                GameObject last = Instantiate(roadDebugLine, parent.transform);
                last.name = string.Format("{0}{1} Road", roadThread.roads[i].roadType.ToString(), i);
                LineRenderer lineRenderer = last.GetComponent<LineRenderer>();
                lineRenderer.positionCount = roadThread.roads[i].points.Length;

                for (int j = 0; j < lineRenderer.positionCount; j++)
                {
                    //This adds the offset for each point
                    lineRenderer.SetPosition(j, roadThread.roads[i].points[j] + roadThread.position);
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
        //Adding to the cities list for help connecting roads
        cities.Add(new City(cityName,roadThread.citySlots,roadThread.roads, roadThread.position));
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

    /*
    //Spawning the terrain loaders and then waiting for them to be completed
    public void SpawnTerrainLoaders()
    {
        foreach (City city in cities)
        {
            GameObject lastTerrainLoader = Instantiate(terrainLoader, city.position, Quaternion.Euler(0, 0, 0));
            lastTerrainLoader.name = city.name + " Terrain Loader";
        }
        //FindObjectOfType<EndlessTerrain>().StartTerrainGeneration();
    }
    //Once the terrain loaders have generated the mesh, then we start flattening it for the roads
    public void FlattenMesh()
    {
        if (loading.totalLoaderChunkActions == loading.numberOfChunkActionsCompleted && !meshFlattened)
        {
            meshFlattened = true;
            Debug.Log("Flattening Mesh");
            //Done loading the chunks for cities
            foreach (City city in cities)
            {
                RaycastHit hit;
                //Debug.Log(string.Format("Checking ground for {0} at location {1}", city.name, city.position));
                if (Physics.Raycast(city.position + new Vector3(0,99,0), -Vector3.up, out hit))
                {
                    if (hit.transform.CompareTag("Terrain"))
                    {
                        MeshCollider meshCollider = hit.collider as MeshCollider;
                        Mesh mesh = meshCollider.sharedMesh;
                        Vector3[] vertices = mesh.vertices;
                        int[] triangles = mesh.triangles;
                        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
                        //We have got that height stored at the center of the town
                        float yHeight = vertices[triangles[hit.triangleIndex * 3 + 0]].y;
                        int centerVertice = triangles[hit.triangleIndex * 3 + 0];
                        //Now we need to apply it to the mesh 20 in each direction
                        try
                        {
                            //20 is the half the distance in vertices we need to go
                            int meshsize = 565;
                            int amountToTravel = 6;
                            for (int y = -amountToTravel; y <= amountToTravel; y++)
                            {
                                for (int xIndex = -amountToTravel; xIndex <= amountToTravel; xIndex++)
                                {
                                    
                                    vertices[centerVertice + 2 +(meshsize * y) + xIndex].y = yHeight; //1
                                    vertices[centerVertice + (meshsize * y) + xIndex].y = yHeight; //2
                                    vertices[centerVertice + 1 + (meshsize * y) + xIndex].y = yHeight; //3
                                    vertices[centerVertice + 3 +(meshsize * y) + xIndex].y = yHeight; //4
                                    vertices[centerVertice + 5 + 1 + (meshsize * y) + xIndex].y = yHeight; //5
                                    vertices[centerVertice + 4 + (meshsize * y) + xIndex].y = yHeight; //6
                                    
                                    vertices[centerVertice + (6 * xIndex) + (meshsize * y)].y = yHeight; //2
                                    vertices[centerVertice + 1 + (6 * xIndex) + (meshsize * y)].y = yHeight; //3
                                    vertices[centerVertice + 2 + (6 * xIndex) + (meshsize * y)].y = yHeight; //1
                                    vertices[centerVertice + 3 + (6 * xIndex) + (meshsize * y)].y = yHeight; //4
                                    vertices[centerVertice + 4 + (6 * xIndex) + (meshsize * y)].y = yHeight; //6
                                    vertices[centerVertice + 5 + (6 * xIndex) + (meshsize * y)].y = yHeight; //5

                                }
                            }
                            


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        mesh.vertices = vertices;
                        GameObject a = new GameObject(city.name);
                        a.transform.position = p0;
                    }
                    else
                    {
                        Debug.Log(string.Format("We hit something else called {0}", hit.transform.name));
                    }
                }
                else
                {
                
                }
            }
        }
    }
                            */
}