using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ColliderCallBack : MonoBehaviour
{
    public EventTrigger.TriggerEvent callBack;
    public Road myRoad;

    private void Awake()
    {
        callBack = new EventTrigger.TriggerEvent();
        gameObject.AddComponent<Rigidbody>().isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RoadCollider"))
        {
            Road otherRoad = other.gameObject.GetComponentInParent<Road>();
            if (otherRoad.roadType.id < myRoad.roadType.id)
            {
                callBack.Invoke(new BaseEventData(EventSystem.current));
            }
        }
    }
}
