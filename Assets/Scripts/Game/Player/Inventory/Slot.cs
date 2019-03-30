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
        inventoryManager.SlotDrag(slotNumber, eventData.button);
        
    }

    IEnumerator HoverTimer()
    {
       yield return new WaitForSeconds(mouseOverTimer);
       inventoryManager.MouseOverSlot(slotNumber);
    }
}
