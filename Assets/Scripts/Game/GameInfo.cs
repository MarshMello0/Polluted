using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    [Min(0)]
    public int seed = 0;


    [HideInInspector] public static GameInfo _instance;

    private void Awake()
    {
        //This script will be on the main menu, store the information that the player selects
        //then will move back to the game scene.
        DDOL();
    }

    private void DDOL()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
    }
}
