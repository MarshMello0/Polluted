using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float hunger;
    public float thirst;
    public float health;

    [SerializeField] private float maxHunger;
    [SerializeField] private float maxThirst;
    [SerializeField] private float maxHealth;
    
    [Space]
    
    [SerializeField] private float idleTime = 1f;

    [Header("User interface")] 
    
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider thirstSlider;
    [SerializeField] private Slider healthSlider;

    private bool hungerLow, thirstLow;

    [HideInInspector] public bool isPaused;
    
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
        while (isPaused)
        {
            yield return null;
        }
        hunger -= 0.3f;
        thirst--;
        UpdateUI();
        StartCoroutine(StatsIdle());
    }

    /// <summary>
    /// This updates the UI, so instead of updating it every frame, just when we need to
    /// </summary>
    private void UpdateUI()
    {
        hungerSlider.value = (hunger / maxHunger) * 100f;
        thirstSlider.value = (thirst / maxThirst) * 100f;
        healthSlider.value = (health / maxHealth) * 100f;
    }

    private void Update()
    {
        if (hunger <= 0 && !hungerLow)
        {
            hungerLow = true;
            StartCoroutine(LowHunger());
        }
        else if (hunger > 0 && hungerLow)
        {
            hungerLow = false;
        }

        if (thirst <= 0 && !thirstLow)
        {
            thirstLow = true;
            StartCoroutine(LowThirst());
        }
        else if (thirst > 0 && thirstLow)
        {
            thirstLow = false;
        }

        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
        thirst = Mathf.Clamp(thirst, 0f, maxThirst);
    }

    IEnumerator LowHunger()
    {
        while (isPaused)
        {
            yield return null;
        }
        health -= 0.5f;
        yield return new WaitForSeconds(idleTime);
        if (hungerLow)
        {
            StartCoroutine(LowHunger());
        }
    }
    
    IEnumerator LowThirst()
    {
        while (isPaused)
        {
            yield return null;
        }
        health -= 0.5f;
        yield return new WaitForSeconds(idleTime);
        if (thirstLow)
        {
            StartCoroutine(LowThirst());
        }
    }
}
