using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggedSlot : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    private Vector3 mouseOffset;
    [SerializeField] private RectTransform rectTransform;
    
    private void OnEnable()
    {
        mouseOffset = Input.mousePosition - rectTransform.position;
    }

    private void Update()
    {
        rectTransform.position = Input.mousePosition + mouseOffset;
    }
}
