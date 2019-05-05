using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomSwitcher : MonoBehaviour
{

    private static readonly int poolSize = 20000;
    
    [SerializeField] private GameObject[] roomPrefabs;
    
    private Loading loading;
    private bool isLoading = true;

    
    
    private void Start()
    {
        loading = FindObjectOfType<Loading>();
    }

    private void Update()
    {
        if (isLoading)
        {
            if (loading == null)
                isLoading = false;
        }
    }

    public void SetRooms()
    {
        StartCoroutine(SpawnRooms());
    }

    IEnumerator SpawnRooms()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
        if (isLoading)
        {
            loading.totalNumberOfActions += rooms.Length + 1;
        }
        
        for (int i = 0; i < rooms.Length; i++)
        {
            Transform room = rooms[i].transform;
            room.tag = "Untagged";
            int childIndex = Random.Range(0, transform.childCount - 1);
            Transform pooledRoom = transform.GetChild(childIndex);
            pooledRoom.SetParent(room);
            pooledRoom.gameObject.SetActive(true);
            if (isLoading)
            {
                loading.numberOfActionsCompleted++;
            }

            if (i % 80 == 0)
            {
                yield return new WaitForEndOfFrame(); //Adds a delay to try and reduce the lag
            }
            
        }
    }
}