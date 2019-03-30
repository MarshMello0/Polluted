using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TMPButton : MonoBehaviour, IPointerClickHandler
{
    public EventTrigger.TriggerEvent callBack;

    public void OnPointerClick(PointerEventData eventData)
    {
        callBack.Invoke(eventData);
    }
}
