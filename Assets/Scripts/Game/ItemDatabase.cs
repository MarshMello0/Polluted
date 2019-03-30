using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items = new List<Item>();

    public Item GetItemByID(int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id)
            {
                return items[i];
            }
        }
        Debug.LogError(string.Format("Couldn't find an item with the ID of {0}", id));
        return null;
    }
    
    public Item GetItemByDisplayName(string displayName)
    {
        //Doing this ToLower so that capital letters don't effect it
        string name = displayName.ToLower();
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].displayName.ToLower() == name)
            {
                return items[i];
            }
        }
        Debug.LogError(string.Format("Coudln't find an item with the display name of {0}", displayName));
        return null;
    }
}
