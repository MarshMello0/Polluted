using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Loading : MonoBehaviour
{
    //This is the total number of things needed to be completed before the player can play
    public int totalNumberOfActions = 0;
    //This is the current number of actions done by the clients machine
    public int numberOfActionsCompleted = 0;
    private DateTime startTime;
    [SerializeField] private Slider slider;
    private void Awake()
    {
        startTime = DateTime.Now;
        CalActions();
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }

    private void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game"));
    }

    private void CalActions()
    {
        //Things to work out
        //How many chunks will need to be loaded from view distance
        int i_viewDistance = FileManager.GetIntValueFromConfig("i_viewdistance");
        
        float maxViewDst =  i_viewDistance * 50;
        float chunkSize = 94;
        int chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize) ;
        int amountOfChunks = ((chunksVisibleInViewDst * 2) + 1) * ((chunksVisibleInViewDst * 2) + 1);
        totalNumberOfActions += amountOfChunks;
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
            DateTime finished = DateTime.Now;
            TimeSpan duration = finished.Subtract(startTime);
            CustomDebug.Log(CustomDebug.Type.Log, string.Format("Finished loading in {0} milliseconds", duration.TotalMilliseconds));
            
            SceneManager.UnloadSceneAsync(1);
        }
    }
}
