using UnityEngine;
using UnityEngine.EventSystems;

public class TMPButton : MonoBehaviour, IPointerClickHandler
{
    public EventTrigger.TriggerEvent callBack;

    public void OnPointerClick(PointerEventData eventData)
    {
        callBack.Invoke(eventData);
    }
}
