using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    private Dictionary<string, GameObject> panels;
    [SerializeField] 
    private TextMeshProUGUI mainTitle;
    [SerializeField]
    private List<GameObject> panelsList;

    [SerializeField] private GameObject mainSection, pauseMenu;

    private KeyCode kPause;
    private bool hasChangedControls;
    
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ControlsUI controlsUI;
    

    private void Start()
    {
        panels = new Dictionary<string, GameObject>();
        for (int x = 0; x < panelsList.Count; x++)
        {
            panels.Add(panelsList[x].name, panelsList[x]);
        }
        SetControls();
    }

    private void SetControls()
    {
        List<Binding> bindings = FileManager.GetPlayersConfigAsBindings();

        for (int i = 0; i < bindings.Count; i++)
        {
            Binding lastBind = bindings[i];
            switch (lastBind.commandName)
            {
                case "pause":
                    kPause = lastBind.binding;
                    return; //that's the only key we are looking for
            }
        }
    }

    private void Update()
    {
        if (!pauseMenu.activeInHierarchy && Input.GetKeyDown(kPause))
        {
            pauseMenu.SetActive(true);
            playerController.inUI = true;
            Mouse.Lock(false);
            Time.timeScale = 0;
        }
        else if (pauseMenu.activeInHierarchy)
        {
            if (Input.GetKeyDown(kPause) || Input.GetKeyDown(KeyCode.Escape))
            {
                Continue();
            }
        }
    }

    public void Exit()
    {
        Enable("Exit");
    }

    public void Options()
    {
        Enable("Options");
        hasChangedControls = true;
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
        if (!mainSection.activeInHierarchy)
            mainSection.SetActive(true);
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

    public void Quit(bool save)
    {
        Debug.Log("Quitting Save = " + save);
    }

    public void CancelQuit()
    {
        DisableAll();
        mainSection.SetActive(false);
    }

    public void Continue()
    {
        mainSection.SetActive(false);
        pauseMenu.SetActive(false);
        Mouse.Lock(true);
        playerController.inUI = false;
        Time.timeScale = 1;
        if (hasChangedControls)
        {
            controlsUI.SaveControls();
            playerController.SetControls();
            hasChangedControls = false;
        }
            
    }
    
    
    
    
}
