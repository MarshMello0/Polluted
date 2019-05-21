using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class InGameTimer : MonoBehaviour
{
    [SerializeField] private SessionStats sessionStats;
    private float currentTime = 0f;
    private void Start()
    {
        sessionStats = GameObject.FindWithTag("GameInfo").GetComponent<SessionStats>();
        SceneManager.sceneUnloaded += UnloadedScene;
        StartCoroutine(GameTimer());
    }

    private void UnloadedScene(Scene arg0)
    {
        if (currentTime > sessionStats.longestPlayTime)
        {
            sessionStats.longestPlayTime = currentTime;
            Debug.Log("The time was greater than the longest play time");
        }
    }

    IEnumerator GameTimer()
    {
        yield return new WaitForSeconds(1);
        currentTime += 1;
        StartCoroutine(GameTimer());
    }
}
