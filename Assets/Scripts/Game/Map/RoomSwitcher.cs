using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] roomPrefabs;
    
    private GameObject[] rooms;
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

    private void FindRooms()
    {
        rooms = GameObject.FindGameObjectsWithTag("Room");
        if (isLoading)
        {
            loading.totalNumberOfActions += rooms.Length;
        }
        
    }

    public void SetRooms()
    {
        FindRooms();
        StartCoroutine(DelaySpawn());
    }

    IEnumerator DelaySpawn()
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            Transform room = rooms[i].transform;
            room.tag = "Untagged";
            GameObject lastRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)], room);
            if (isLoading)
            {
                loading.numberOfActionsCompleted++;
            }
            yield return new WaitForEndOfFrame(); //Adds a delay to try and reduce the lag
        }
        rooms = new GameObject[0];
    }
}