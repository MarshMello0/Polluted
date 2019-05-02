using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public float hunger;
    public float thirst;

    [SerializeField] private float maxHunger;
    [SerializeField] private float maxThirst;
    [SerializeField] private float idleTime = 1f;

    [Header("User interface")] 
    [SerializeField] private TextMeshProUGUI statsText;

    private void Start()
    {
        StartCoroutine(StatsIdle());
    }

    /// <summary>
    /// This is the slow idle of the players hunger and thirst
    /// </summary>
    IEnumerator StatsIdle()
    {
        yield return new WaitForSecondsRealtime(idleTime);
        hunger--;
        thirst--;
        UpdateUI();
        StartCoroutine(StatsIdle());
    }

    /// <summary>
    /// This updates the UI, so instead of updating it every frame, just when we need to
    /// </summary>
    private void UpdateUI()
    {
        statsText.text =
            string.Format("Hunger: {0}% Thirst: {1}%", (hunger / maxHunger) * 100, (thirst / maxThirst) * 100);
    }
}
