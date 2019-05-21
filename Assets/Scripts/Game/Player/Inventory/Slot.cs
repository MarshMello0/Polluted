using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private float mouseOverTimer = 1f;
    [SerializeField] private InventoryManager inventoryManager;
    public int slotNumber;    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(HoverTimer());
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        inventoryManager.MouseLeftSlot(slotNumber);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(Click(eventData));
    }

    IEnumerator Click(PointerEventData eventData)
    {
        float waitTime = 0.15f;
        float currentTime = 0;
        while (currentTime < waitTime)
        {
            if (!Input.GetMouseButton((int)eventData.button))
            {
                inventoryManager.SetItemInfo(true, slotNumber);
                StopAllCoroutines();
            }
            currentTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        inventoryManager.SlotDrag(slotNumber, eventData.button);
        yield return null;
    }

    IEnumerator HoverTimer()
    {
       yield return new WaitForSeconds(mouseOverTimer);
       inventoryManager.MouseOverSlot(slotNumber);
    }
}
