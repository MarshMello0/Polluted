using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    /*
     * When the player tries to pick up an item
     * it sends a raycast from the camera and if it hits a game object with a tag
     *
     * For the UI of what the item is, on the item there will be a collider
     * If that collider collides with the player then enable the UI with the information about
     */

    [SerializeField] private KeyCode kInteract;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InventoryManager inventoryManager;
    
    private void Update()
    {
        if (Input.GetKeyDown(kInteract))
        {
            TryPickUpItem();
        }
    }

    private void TryPickUpItem()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 10))
        {
            string tag = hit.transform.tag;

            switch (tag)
            {
                case "Pickupable":
                    PickableItem item = hit.transform.GetComponent<PickableItem>();
                    inventoryManager.AddItem(item.id, 1);
                    Destroy(hit.transform.gameObject);
                    break;
            }
        }
    }
}
