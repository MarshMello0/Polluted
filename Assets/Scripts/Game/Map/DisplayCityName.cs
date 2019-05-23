using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayCityName : MonoBehaviour
{
    // Start is called before the first frame update
    private string cityName;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject textGO;
    [SerializeField] private PauseMenuManager pauseMenuManager;
    void Start()
    {
        cityName = GameObject.FindWithTag("City").name; 
    }

    public void DisplayName()
    {
        StartCoroutine(DispalyText("Welcome to " + cityName));
    }
    
    private IEnumerator DispalyText(string text)
    {
        this.text.text = "";
        List<string> characters = new List<string>(new string[text.Length]);
        foreach (char letter in text)
        {
            while (pauseMenuManager.isPaused)
            {
                yield return null;
            }
            this.text.text += letter;
            characters.Insert(0,letter.ToString());
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(5);


        foreach (char c in this.text.text)
        {
            this.text.text = this.text.text.Remove(0, 1);
            yield return new WaitForSeconds(0.1f);
        }
        
        
        
        
        textGO.SetActive(false);
        enabled = false;
    }
}
