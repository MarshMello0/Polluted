using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private GameObject roadDebugLine;
    private RoadThread roadThread;
    
    private void Start()
    {
        roadThread = new RoadThread();
        roadThread.log = Log;
        roadThread.Start();
        StartCoroutine(WaitForThread());
    }

    private IEnumerator WaitForThread()
    {
        //This will wait for that thread to be complete
        yield return StartCoroutine(roadThread.WaitFor());

        for (int i = 0; i < roadThread.roads.Count; i++)
        {
            GameObject last = Instantiate(roadDebugLine);
            last.name = string.Format("{0}{1} Road", roadThread.roads[i].roadType.ToString(),i);
            LineRenderer lineRenderer = last.GetComponent<LineRenderer>();
            lineRenderer.positionCount = roadThread.roads[i].points.Length;
            lineRenderer.SetPositions(roadThread.roads[i].points);

            if (roadThread.roads[i].roadType == RoadType.M)
            {
                lineRenderer.startColor = Color.blue;
                lineRenderer.endColor = Color.blue;
            }
            else if (roadThread.roads[i].roadType == RoadType.A)
            {
                lineRenderer.startColor = Color.green;
                lineRenderer.endColor = Color.green;
            }
        }
    }

    public void Log(string message)
    {
        Debug.Log(message);
    }
}