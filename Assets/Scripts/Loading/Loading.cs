using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Loading : MonoBehaviour
{
    //This is the total number of things needed to be completed before the player can play
    private int totalNumberOfActions = 0;
    //This is the current number of actions done by the clients machine
    public int numberOfActionsCompleted = 0;
    private DateTime startTime;
    private void Awake()
    {
        startTime = DateTime.Now;
        CalActions();
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
        Time.timeScale = 0;
        
    }

    private void CalActions()
    {
        //Things to work out
        //How many chunks will need to be loaded from view distance
        int i_viewDistance = FileManager.GetIntValueFromConfig("i_viewdistance");
        
        float maxViewDst =  i_viewDistance * 300;
        float chunkSize = 94;
        int chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize) ;
        int amountOfChunks = ((chunksVisibleInViewDst * 2) + 1) * ((chunksVisibleInViewDst * 2) + 1);
        CustomDebug.Log(CustomDebug.Type.Log, string.Format("Loading.cs : {0}",chunksVisibleInViewDst));
        totalNumberOfActions += amountOfChunks;
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


    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2);
        

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            CustomDebug.Log(CustomDebug.Type.Log, "Loading Scene " + (asyncLoad.progress * 100) + "%");
            yield return null;
        }
        CustomDebug.Log(CustomDebug.Type.Log,"Done Loading");
    }
}
