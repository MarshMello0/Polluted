using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public Transform connectedPoint;

    public void AddConnection(Transform connectionPoint)
    {
        connectedPoint = connectionPoint;
    }
}
