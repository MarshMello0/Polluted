using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IdleTimer : MonoBehaviour
{
    [SerializeField] private float idleTimeout = 5f;
    private bool idleTimerRunning;
    private Coroutine timer;

    private void Update()
    {
        if (!idleTimerRunning)
        {
            timer = StartCoroutine(Idle());
        }
    }

    public void Active()
    {
        StopCoroutine(timer);
        idleTimerRunning = false;
    }
    
    private IEnumerator Idle()
    {
        idleTimerRunning = true;
        yield return new WaitForSecondsRealtime(idleTimeout);
        Mouse.Lock(false);
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
