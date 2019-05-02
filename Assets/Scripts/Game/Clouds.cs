using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    [SerializeField] private GameObject[] clouds;
    [SerializeField] private GameObject[] cloudClusters;


    [SerializeField] private List<GameObject> activeClouds = new List<GameObject>();
    [SerializeField] private List<GameObject> pooledClouds = new List<GameObject>();

    [SerializeField] private Transform player;
    [SerializeField] private Transform cloudsParent;
    [SerializeField] private Transform cloudsCentre;
    [SerializeField] private Transform cloudsRotator;

    [Space] 
    
    //This direction will be what is used to move the clouds
    public Vector2 windDirection;
    [SerializeField] private int cloudHeight;
    [SerializeField] private float minCloudScale, maxCloudScale;
    [SerializeField] private float maxDistance;
    [SerializeField] private float activeCloudCount = 50;

    private bool finishedLoading;
    
    private void Start()
    {
        GetPooledClouds();
        StartingClouds();
        cloudsCentre.localPosition = new Vector3(0,-maxDistance,0);
    }
    
    private void Update()
    {
        if (!finishedLoading)
        {
            if (player.gameObject.activeInHierarchy)
            {
                finishedLoading = true;
            }
            else return;
        }
        
        MoveClouds();
        CheckClouds();
    }

    /// <summary>  
    /// This just gets all of the child gameobjects of the parent
    /// and puts them into the pool
    /// </summary> 
    private void GetPooledClouds()
    {
        for (int i = 0; i < cloudsParent.childCount; i++)
        {
            pooledClouds.Add(cloudsParent.GetChild(i).gameObject);
            cloudsParent.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    /// <summary>  
    ///This will spawn the first clouds at random above the players had
    /// once the first lot have spawned then we just need to spawn ones 
    ///at the start of the direction, out of the players view distance.
    /// </summary>  
    private void StartingClouds()
    {
        for (int i = 0; i < activeCloudCount; i++)
        {
            float x = Random.Range(-maxDistance, maxDistance);
            float z = Random.Range(-maxDistance, maxDistance);
            SpawnCloud(new Vector2(x,z));
        }
    }

    /// <summary>  
    ///  This is the spawner which is used once the clouds have got going,
    /// this will spawn the cloud out of the players view distance
    /// and it will slowly move across the scene
    /// </summary>  
    private void Spawner()
    {
        
    }
    
    /// <summary>  
    ///  This is the main interaction with the pooling system, this will
    /// check if there is enough and if there isn't it will spawn a new one
    /// and add it to the pool
    /// </summary>  
    private void SpawnCloud(Vector2 pos)
    {
        float scale = Random.Range(minCloudScale, maxCloudScale);
        GameObject cloud = null;
        if (pooledClouds.Count > 1)
        {
            cloud = pooledClouds[0];
            pooledClouds.RemoveAt(0);
            cloud.SetActive(true);
            
        }
        else
        {
            //If the pool is empty, lets spawn a new cloud
            
            //If its 1, its a cloud, if its 2 its a cluster
            if (Random.Range(0, 1) == 0)
            {
                cloud = Instantiate(clouds[Random.Range(0,clouds.Length - 1)]);
            }
            else
            {
                cloud = Instantiate(cloudClusters[Random.Range(0, cloudClusters.Length - 1)]);
            }
            
        }
        activeClouds.Add(cloud);
        cloud.transform.position = new Vector3(pos.x, cloudHeight, pos.y);
        cloud.transform.localScale = new Vector3(scale,scale,scale);
    }

    private void MoveClouds()
    {
        for (int i = 0; i < activeClouds.Count; i++)
        {
            activeClouds[i].transform.position += new Vector3(windDirection.x,0,windDirection.y);
        }
    }

    /// <summary>
    /// This checks to see if any clouds are out of the bounds
    /// </summary>
    private void CheckClouds()
    {
        cloudsRotator.LookAt(new Vector3(-windDirection.x,cloudHeight,-windDirection.y));
        for (int i = 0; i < activeClouds.Count; i++)
        {
            if (Vector3.Distance(activeClouds[i].transform.localPosition, Vector3.zero) > maxDistance)
            {
                GameObject cloud = activeClouds[i];
                activeClouds.RemoveAt(i);
                pooledClouds.Add(cloud);
                cloud.SetActive(false);
                Vector2 newCloudPos = new Vector2(-cloud.transform.localPosition.x, -cloud.transform.localPosition.z);
                Debug.Log(-cloud.transform.localPosition);
                SpawnCloud(newCloudPos);
                cloud.transform.localPosition = new Vector3(0,-1000,0);
                
            }
        }
    }
    
}
