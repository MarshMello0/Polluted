using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int inventorySize;
    [SerializeField] private List<Item> slots = new List<Item>();
    [SerializeField] private List<int> amounts = new List<int>();

    [SerializeField] private ItemDatabase itemDataBase;

    [Header("UI Items")] 
    [SerializeField] private GameObject backPanel;
    private Dictionary<UIType, GameObject> panels = new Dictionary<UIType, GameObject>();
    [SerializeField] private Transform itemSlotsHolder;

    [Header("Tool Tip Items")] 
    [SerializeField] private RectTransform toolTipHolder;
    [SerializeField] private Vector3 toolTipOffset;
    [SerializeField] private TextMeshProUGUI toolTipName, toolTipDescription;
    private int toolTipLastSlot = -1;
    private List<Transform> slotsTransform = new List<Transform>();

    [Header("Dragging Items")] 
    [SerializeField] private RectTransform dragSlot;
    [SerializeField] private Image dragSlotImage;
    [SerializeField] private TextMeshProUGUI dragSlotAmount;
    [SerializeField] private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    [SerializeField] private EventSystem m_EventSystem;

    private Item lastDraggedItem;
    private int lastDraggedAmount;
    private int lastDraggedSlot;
    private bool isDragging;
    
    public enum UIType
    {
        Inventory, Crafting,Map,Storage
    }

    private void Start()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new Item());
            amounts.Add(0);
        }

        for (int i = 0; i < itemSlotsHolder.childCount; i++)
        {
            slotsTransform.Add(itemSlotsHolder.GetChild(i));
        }
        
        AddItem(1,19);
        AddItem(1,10);
        AddItem(1,1);
    }

    private void Update()
    {
        toolTipHolder.position = Input.mousePosition + toolTipOffset;
        
        Dragging();
    }

    public void AddItem(int id, int amount)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].id == -1)
            {
                slots[i] = itemDataBase.GetItemByID(id);
                amounts[i] = amount;
                Debug.Log(string.Format("Added item into player inventory at slot {0}", i));
                return;
            }
        }
        Debug.Log("Couldn't find a free slot to place the item in.");
        if (backPanel.activeInHierarchy)
        {
            UpdateUI();
        }
    }   
    public void AddItem(int id, int amount, int slot)
    {
        if (slots[slot].id != -1)
        {
            Debug.LogError("There is an item there");
            return;
        }
        slots[slot] = itemDataBase.GetItemByID(id);
        amounts[slot] = amount;
        if (backPanel.activeInHierarchy)
        {
            UpdateUI();
        }
    }

    public void RemoveItem(int slot)
    {
        slots[slot] = new Item();
        if (backPanel.activeInHierarchy)
        {
            UpdateUI();
        }
    }

    public void ClearInventory()
    {
        slots.Clear();
        slots.TrimExcess();
        amounts.Clear();
        amounts.TrimExcess();
        
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new Item());
            amounts.Add(0);
        }
    }

    public Sprite GetItem(int slot)
    {
        return slots[slot].image;
    }

    public void OpenUI(UIType uiType)
    {
        backPanel.SetActive(true);
        DisableAllUI();
        UpdateUI();
        switch (uiType)
        {
            case UIType.Inventory:
                break;
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Image image = slotsTransform[i].GetChild(0).GetComponent<Image>();
            if (slots[i].id != -1)
            {
                image.sprite = slots[i].image;
                image.color = new Color(1,1,1,1);
            }
            else
            {
                image.color = new Color(1,1,1,0);
            }
            slotsTransform[i].GetChild(1).GetComponent<TextMeshProUGUI>().text = amounts[i].ToString();
        }
    }

    public void CloseUI()
    {
        DisableAllUI();
        backPanel.SetActive(false);
    }
    

    private void DisableAllUI()
    {
        foreach (KeyValuePair<UIType,GameObject> panel in panels)
        {
            panels[panel.Key].SetActive(false);
        }
        
        //closes the tool tip if there is one open
        if (toolTipLastSlot != -1)
        {
            MouseLeftSlot(toolTipLastSlot); 
        }
    }

    public void MouseOverSlot(int slotNumber)
    {
        if (slots[slotNumber].id != -1)
        {
            toolTipHolder.gameObject.SetActive(true);
            toolTipName.text = slots[slotNumber].displayName;
            toolTipDescription.text = slots[slotNumber].description;
            toolTipLastSlot = slotNumber;
        }
    }

    public void MouseLeftSlot(int slotNumber)
    {
        if (slotNumber == toolTipLastSlot)
        {
            toolTipHolder.gameObject.SetActive(false);
            toolTipLastSlot = -1;
        }
    }

    public void SlotDrag(int slotNumber, PointerEventData.InputButton inputButton)
    {
        //Checking if there is anything in that slot 
        if (slots[slotNumber].id == -1)
        {
            return;
        }
        //There must be something there
        lastDraggedItem = slots[slotNumber];
        lastDraggedSlot = slotNumber;
        
        //Working out if we can half it
        /*
         * amount = the amount we have of that item
         * a = amount / 2 rounded down
         * b = the different if amount / 2 is not a whole. But it only returns hole numbers "Remainder operator %"
         * c = amount halfed + the difference
         */
        int amount = amounts[slotNumber];
        amounts[slotNumber] = 0;
        int a = Mathf.FloorToInt(amount / 2f);
        int b = amount % 2;
        int c = a + b; 
        
        if (inputButton == PointerEventData.InputButton.Right)
        {
            amounts[slotNumber] = a;
            lastDraggedAmount = c;
            //This will be true if we try and half 1
            if (a == 0)
            {
                slots[slotNumber] = new Item();
            }
        }
        else
        {
            lastDraggedAmount = amount;
            slots[slotNumber] = new Item();
            
        }
        
        UpdateUI();
        
        
        dragSlot.position = itemSlotsHolder.GetChild(slotNumber).GetComponent<RectTransform>().position;
        dragSlot.gameObject.SetActive(true);
        dragSlotImage.sprite = lastDraggedItem.image;
        dragSlotAmount.text = lastDraggedAmount.ToString();
        isDragging = true;
    }

    private void Dragging()
    {
        if (!isDragging) return;
        
        bool isSplitting = Input.GetMouseButtonUp(1);

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            dragSlot.gameObject.SetActive(false);
            
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("Slot"))
                {
                    Slot slot = result.gameObject.GetComponent<Slot>();
  
                    //Checking if the items match id
                    if (slots[slot.slotNumber].id == lastDraggedItem.id)
                    {
                        int newSize = lastDraggedAmount + amounts[slot.slotNumber];
                        int maxStackSize = itemDataBase.GetItemByID(lastDraggedItem.id).maxStackSize;
                        if (newSize > maxStackSize)
                        {
                            int previousAmount = amounts[slot.slotNumber];
                            int leftOver = lastDraggedAmount - (maxStackSize - previousAmount);
                            amounts[slot.slotNumber] = maxStackSize;
                            
                            //If we are spliiting into a full stack we dont want to add back, just set the value
                            if (isSplitting)
                            {
                                amounts[lastDraggedSlot] += leftOver;
                            }
                            else
                            {
                                AddItem(lastDraggedItem.id, leftOver, lastDraggedSlot);
                            }
                        }
                        else
                        {
                            amounts[slot.slotNumber] = newSize;
                        }
                    }
                    else
                    {
                        //If we are splitting a stack we want to just send it back as it can't be placed
                        //If we are not splitting then we can switch items
                        Item switchItem = slots[slot.slotNumber];
                        int switchAmount = amounts[slot.slotNumber];
                        if (!isSplitting)
                        {
                            
                            //Resetting that slot
                            slots[slot.slotNumber] = new Item();
                            amounts[slot.slotNumber] = 0;
                            //Adding the new item in
                            AddItem(lastDraggedItem.id, lastDraggedAmount, slot.slotNumber);
                            //Adding the item we switched with
                            if (switchItem.id != -1)
                            {
                                //This if is just to check if there is an item in that slot
                                //because they could switch with an empty slot causing an error
                                AddItem(switchItem.id,switchAmount,lastDraggedSlot);
                            }
                        }
                        else if (switchItem.id == -1)
                        {
                            AddItem(lastDraggedItem.id, lastDraggedAmount, slot.slotNumber);
                        }
                        else
                        {
                            //Send it back
                            amounts[lastDraggedSlot] += lastDraggedAmount;
                        }
                    }
                    
                    

                    
                    
                    //Clearing the last dragged item
                    lastDraggedItem = null;
                    lastDraggedSlot = -1;
                    lastDraggedAmount = 0;
            
                    isDragging = false;
                    UpdateUI();
                    return;
                }
                
                if (result.gameObject.CompareTag("DropItem") && results.Count == 1)
                {
                    Debug.Log(string.Format("Item {0} has been dropped", lastDraggedItem.displayName));
                    //Clearing the last dragged item
                    lastDraggedItem = null;
                    lastDraggedSlot = -1;
                    lastDraggedAmount = 0;
            
                    isDragging = false;
                    
                    return;
                }
            }
            
            //They must of missed a slot, so lets place it back where it came from
            if (isSplitting)
            {
                amounts[lastDraggedSlot] += lastDraggedAmount;
            }
            else
            {
                AddItem(lastDraggedItem.id,lastDraggedAmount,lastDraggedSlot);
            }
            lastDraggedItem = null;
            lastDraggedSlot = -1;
            lastDraggedAmount = 0;
            UpdateUI();
            isDragging = false;

        }
    }
    
    
}

[System.Serializable]
public class Item
{
    public string displayName;
    public int id;
    public int maxStackSize;
    public string description;
    public Sprite image;

    public Item()
    {
        this.displayName = "Blank";
        this.id = -1;
        this.maxStackSize = -1;
        this.description = "This is a blank item";
        this.image = null;
    }
    public Item(string displayName, int id,int maxStackSize , string description)
    {
        this.displayName = displayName;
        this.id = id;
        this.maxStackSize = maxStackSize;
        this.description = description;
        this.image = null;
    }
}