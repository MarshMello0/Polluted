using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This script is what spawns the rooms and items in those rooms around the player
/// </summary>
public class Spawner : MonoBehaviour
{

    [SerializeField] private Transform itemsParent;
    [SerializeField] private GameObject[] roomPrefabs;
    [SerializeField] private ItemDatabase itemDatabase;
    private void OnTriggerEnter(Collider other)
    {
        Transform transform = other.transform;
        string tag = other.tag;
        StartCoroutine(SpawnRoom(tag, transform));
    }

    private IEnumerator SpawnRoom(string tag, Transform transform)
    {
        yield return new WaitForEndOfFrame();
        switch (tag)
        {
            case "Room":
                //Here this is the first time this room has met the player
                //So we pick a random room to spawn and store what number we spawned, 
                //in the game object name
                try
                {
                    int randomRoom = Random.Range(0, roomPrefabs.Length);
                    Instantiate(roomPrefabs[randomRoom], transform);
                    transform.gameObject.name = randomRoom.ToString();
                }
                catch (Exception e)
                {

                }
                
                transform.tag = "LoadedRoom";
                break;
            case "LoadedRoom":
                //If these both are true that means,
                //We have been here before and we want to turn on the room again
                transform.GetChild(0).gameObject.SetActive(true);
                break;
            case "ItemSpawn":
                int randomItem = Random.Range(0, itemDatabase.items.Count - 1);
                int randomPrefab = 0;
                if (itemDatabase.items[randomItem].prefabs.Length != 0)
                {
                    randomPrefab = Random.Range(0, itemDatabase.items[randomItem].prefabs.Length - 1);
                    Instantiate(itemDatabase.items[randomItem].prefabs[randomPrefab], itemsParent).transform.position = transform.position;
                }
                
                transform.tag = "Untagged";
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Transform transform = other.transform;
        if (transform.CompareTag("LoadedRoom"))
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
