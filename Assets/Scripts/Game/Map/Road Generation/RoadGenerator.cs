using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class RoadGenerator : MonoBehaviour
{
    [Header("Road Types")] 
    [SerializeField] private RoadType[] roadTypes;
    [Header("Roads")]
    //List of all of the roads
    [SerializeField] private List<Road> roads = new List<Road>();

    private GameObject go;
   

    private void Start()
    {
        GenerateMap();
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 200, 30), "Generate New Roads"))
            GenerateMap();
    }

    private void GenerateMap()
    {
        if (go != null)
        {
            roads.Clear();
            Destroy(go);
        }
        go = new GameObject("Generated Roads");
        Vector3 startingPosition = Vector3.zero;
        for (int i = 0; i < roadTypes.Length; i++)
        {
            for (int j = 0; j < roadTypes[i].amount; j++)
            {
                if (roads.Count > 0 && roadTypes[i].id != 0)
                {
                    int randomRoad = Random.Range(0, roads.Count - 1);
                    while (roads[randomRoad].roadType.id != roadTypes[i].id - 1)
                    {
                        randomRoad = Random.Range(0, roads.Count - 1);
                    }
                    
                    int randomPoint = Random.Range(0, roads[randomRoad].points.Count - 1);
                    startingPosition = roads[randomRoad].points[randomPoint].position;
                    
                }
                GenerateRoad(startingPosition, Random.Range(0f,360f),roadTypes[i]);
            }
        }
    }
    

    private void Update()
    {
        for (int i = 0; i < roads.Count; i++)
        {
            for (int j = 0; j < roads[i].points.Count - 1; j++)
            {
                Debug.DrawLine(roads[i].points[j].position,roads[i].points[j + 1].position,roads[i].roadType.debugColour);
            }
        }
    }

    private void GenerateRoad(Vector3 startingPoint, float angle, RoadType roadType)
    {
        
        
        GameObject start = new GameObject(roadType.name);
        Road currentRoad = start.AddComponent<Road>();
        
        currentRoad.roadType = roadType;
        start.transform.position = startingPoint;
        start.transform.SetParent(go.transform);
        currentRoad.points = new List<Transform>();
        currentRoad.points.Add(start.transform);
        
        Vector3 newPoint = startingPoint;
        
        for (int i = 0; i < roadType.amountOfPoints; i++)
        {
            //Getting the new range of the angle
            float lowerAngle = angle - (roadType.maxTurnAngle / 2);
            float higherAngle = angle + (roadType.maxTurnAngle / 2);
            //This picks a angle inbetween the two angles
            float chosenAngle = Random.Range(lowerAngle, higherAngle);
            //over riding for the next point
            angle = chosenAngle;
            //This is how far the next point will be
            float chosenDistance = Random.Range(roadType.minPointDistance, roadType.maxPointDistance);
            
            float x = chosenDistance * Mathf.Cos(chosenAngle * Mathf.Deg2Rad);
            float y = chosenDistance * Mathf.Sin(chosenAngle * Mathf.Deg2Rad);

            newPoint.x += x;
            newPoint.z += y;
        
            GameObject lastPoint = new GameObject(i.ToString());
            lastPoint.transform.position = newPoint;
            lastPoint.transform.SetParent(start.transform);
            currentRoad.points.Add(lastPoint.transform);
            
            //Setting the colliders
            GameObject collider = new GameObject("Collider");
            collider.tag = "RoadCollider";
            collider.transform.SetParent(lastPoint.transform);
            //Adding the box collider to that section
            BoxCollider boxCollider = collider.AddComponent<BoxCollider>();
            
            //Adding the script to call back if it collides with something
            ColliderCallBack callBack = collider.AddComponent<ColliderCallBack>();
            callBack.myRoad = currentRoad;
            int a = i;
            callBack.callBack.AddListener(delegate
            {
                currentRoad.ColliderHit(a);
            });
            
            //Getting the center in local space
            Vector3 colliderPoint = new Vector3(lastPoint.transform.position.x - (x / 2f),0,lastPoint.transform.position.z - (y / 2f));
            collider.transform.position = colliderPoint;
            collider.transform.LookAt(lastPoint.transform);
            
            boxCollider.size = new Vector3(1,1,chosenDistance);
            boxCollider.isTrigger = true;

        }
        
        
        roads.Add(currentRoad);
        
        
    }
    
}
[System.Serializable]
public class RoadType
{
    //Just a way to know what it is
    public string name;
    //This variable is the angle in which the next points can be at, eg the max curve of a road
    public float maxTurnAngle;
    //This is the max distance that the next point can be at
    public float maxPointDistance;
    public float minPointDistance;
    //This is how long the road should be in points
    public int amountOfPoints;
    //This is the amount of these roads we want in the world
    public int amount;
    //This is the colour for debugging
    public Color debugColour;
    //This number is used to see what order the roads should go when connecting
    public int id;
}