using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using Debug = UnityEngine.Debug;

public class PickableItem : MonoBehaviour
{
    public int id;
    
    [SerializeField] private GameObject itemInfo;
    [SerializeField] private TextMeshPro text;
    [SerializeField] private ItemDatabase itemDatabase;

    private bool isEnabled;
    private Transform player;
    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        isEnabled = false;
        itemInfo.SetActive(false);

        Item thisItem = null;
        foreach (Item item in itemDatabase.items)
        {
            if (item.id == id)
            {
                thisItem = item;
                break;
            }
        }

        text.text = string.Format("<align=center>{0}\n{1}</align>",thisItem.displayName,thisItem.description);
    }

    private void LateUpdate()
    {
        if (isEnabled)
        {
            itemInfo.transform.LookAt(player);
            Vector3 newPos = transform.position;
            newPos.y = transform.position.y + 0.3f;
            itemInfo.transform.position = new Vector3(newPos.x, newPos.y, newPos.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;
        switch (layer)
        {
            case 13:
                isEnabled = true;
                itemInfo.SetActive(true);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        int layer = other.gameObject.layer;
        switch (layer)
        {
            case 13:
                isEnabled = false;
                itemInfo.SetActive(false);
                break;
        }
    }
}
