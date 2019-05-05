using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Loading : MonoBehaviour
{
    //This is the total number of things needed to be completed before the player can play
    public int totalNumberOfActions;
    //This is the current number of actions done by the clients machine
    public int numberOfActionsCompleted = 1;
    private DateTime startTime;
    [SerializeField] private Slider slider;
    private void Awake()
    {
        totalNumberOfActions = 0;
        numberOfActionsCompleted = 0;
        startTime = DateTime.Now;
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }

    private void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game")); 
    }
    
    private void FixedUpdate()
    {
        float a = numberOfActionsCompleted;
        float b = totalNumberOfActions;
        slider.value =  a / b;
    }

    private void LateUpdate()
    {
        if (totalNumberOfActions == numberOfActionsCompleted)
        {
            Debug.Log(string.Format("{0} {1}", totalNumberOfActions, numberOfActionsCompleted));
            DateTime finished = DateTime.Now;
            TimeSpan duration = finished.Subtract(startTime);
            CustomDebug.Log(CustomDebug.Type.Log, string.Format("Finished loading in {0} milliseconds", duration.TotalMilliseconds));
            SceneManager.UnloadSceneAsync(1);
        }
    }
}


