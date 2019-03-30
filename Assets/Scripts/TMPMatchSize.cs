using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPMatchSize : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("This is the size of what you want all of the texts to be set to.")]
    public float targetSize;

    [SerializeField]
    private TextMeshProUGUI[] texts;

    private void Start()
    {
        UpdateSizes();
    }

    [ContextMenu("Update Sizes")]
    public void UpdateSizes()
    {
        foreach (TextMeshProUGUI text in texts)
        {
            text.fontSize = targetSize;
        }
    }
}