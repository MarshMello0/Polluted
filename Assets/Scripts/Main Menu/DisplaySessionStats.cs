using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplaySessionStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private SessionStats sessionStats;
    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        sessionStats = GameObject.FindWithTag("GameInfo").GetComponent<SessionStats>();
        text.text = string.Format("<align=center>Longest Play Time\n{0} seconds \nTotal Picked Up Items\n{1}</align>",
            sessionStats.longestPlayTime, sessionStats.totalPickedUpItems);
    }
}
