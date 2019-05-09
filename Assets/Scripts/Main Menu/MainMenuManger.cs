using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class MainMenuManger : MonoBehaviour
{
    private Dictionary<string, GameObject> panels;
    [SerializeField] 
    private TextMeshProUGUI mainTitle;
    [SerializeField]
    private List<GameObject> panelsList;
    private void Start()
    {
        panels = new Dictionary<string, GameObject>();
        for (int x = 0; x < panelsList.Count; x++)
        {
            panels.Add(panelsList[x].name, panelsList[x]);
        }
        Enable("Home");
    }

    public void Home()
    {
        Enable("Home");
    }
    public void Play()
    {
        Enable("Play");
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Options()
    {
        Enable("Options");
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }

    private void DisableAll()
    {
        foreach(KeyValuePair<string, GameObject> entry in panels)
        {
            panels[entry.Key].SetActive(false);
        }
    }

    private void Enable(string key)
    {
        DisableAll();
        if (panels.ContainsKey(key))
        {
            panels[key].SetActive(true);
            mainTitle.text = key;
        }
        else
        {
            Debug.LogError(string.Format("{0} Key was not found", key));
        }
    }
}
