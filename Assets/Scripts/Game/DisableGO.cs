using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableGO : MonoBehaviour
{
    private Transform player;
    private float disableDistance = 600;
    
    private bool isEnabled = true;

    private List<GameObject> kids = new List<GameObject>();

    private bool foundKids = false;
    private void Update()
    {
        if (SceneManager.sceneCount == 2)
        {
            return;
        }
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
            return;
        }
        else if (!foundKids)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                kids.Add(transform.GetChild(i).gameObject);
            }

            foundKids = true;
            return;
        }
        
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance > disableDistance && isEnabled)
        {
            foreach (GameObject kid in kids)
            {
                kid.SetActive(false);
            }
            isEnabled = false;
        }
        else if (distance < disableDistance && !isEnabled)
        {
            foreach (GameObject kid in kids)
            {
                kid.SetActive(true);
            }
            isEnabled = true;
        }
    }
}
