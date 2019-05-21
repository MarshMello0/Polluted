using System.Collections;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class TMPButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public EventTrigger.TriggerEvent callBack;
    private TextMeshProUGUI text;
    
    private Color selectedColor = Color.yellow;
    private Color defaultColor;
    private Color clickedColor = Color.cyan;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        defaultColor = text.color;
    }

    private IEnumerator SelectColorDelay()
    {
        text.color = clickedColor;
        yield return new WaitForSeconds(1);
        text.color = defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(SelectColorDelay());
        callBack.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = selectedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = defaultColor;
    }
}
