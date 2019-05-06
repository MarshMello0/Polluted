using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is what spawns the rooms and items in those rooms around the player
/// </summary>
public class Spawner : MonoBehaviour
{
    
    [SerializeField] private GameObject[] roomPrefabs;
    private void OnTriggerEnter(Collider other)
    {
        Transform room = other.transform;
        
        if (other.CompareTag("Room"))
        {
            //Here this is the first time this room has met the player
            //So we pick a random room to spawn and store what number we spawned, 
            //in the game object name
            int randomRoom = Random.Range(0, roomPrefabs.Length - 1);
            Instantiate(roomPrefabs[randomRoom], room);
            room.gameObject.name = randomRoom.ToString();
            room.tag = "LoadedRoom";
        }
        else if (other.CompareTag("LoadedRoom"))
        {
            //If these both are true that means,
            //We have been here before and we want to turn on the room again
            room.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Transform room = other.transform;
        if (room.CompareTag("LoadedRoom"))
        {
            room.GetChild(0).gameObject.SetActive(false);
        }
    }
}
