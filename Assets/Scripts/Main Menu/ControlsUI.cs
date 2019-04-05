using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;

public class ControlsUI : MonoBehaviour
{
    [Header("Bindings")]
    [SerializeField] private List<Binding> keyBindings = new List<Binding>();

    [Space]
    
    [Header("Default Bindings")] 
    [SerializeField] private DefaultBindings defaultBindings;

    [Space]
    
    [Header("Parents")] 
    [SerializeField] private Transform actionHolder;
    [SerializeField] private Transform keyHolder;
    [SerializeField] private Transform mouseHolder;

    [Space] 
    [Header("Press Any Key GameObject")] 
    [SerializeField] private GameObject pak;

    private bool isChangingKey;
    private string keyChanging;

    private void Start()
    {
        LoadPlayersConfig();
    }
    
    public void LoadDefaultBindings()
    {
        keyBindings.Clear();
        for (int i = 0; i < defaultBindings.defaultBindings.Count; i++)
        {
            Binding lastBind = new Binding(
                defaultBindings.defaultBindings[i].displayName,
                defaultBindings.defaultBindings[i].commandName,
                defaultBindings.defaultBindings[i].binding
                );
            keyBindings.Add(lastBind);
        }
        ReloadUI();
    }

    public void LoadPlayersConfig()
    {
        keyBindings.Clear();
        List<Binding> bindings = FileManager.GetPlayersConfigAsBindings();
        for (int i = 0; i < bindings.Count; i++)
        {
            Binding lastBind = new Binding(
                bindings[i].displayName,
                bindings[i].commandName,
                bindings[i].binding
                );
            keyBindings.Add(lastBind);
        }
        ReloadUI();
    }

    private void ReloadUI()
    {      
        for (int i = 0; i < keyBindings.Count; i++)
        {
            Binding lastBinding = keyBindings[i];
            GameObject lastAction = actionHolder.Find(lastBinding.commandName).gameObject;
            GameObject lastKey = keyHolder.Find(lastBinding.commandName).gameObject;
            GameObject lastMouse = mouseHolder.Find(lastBinding.commandName).gameObject;

            lastAction.GetComponent<TextMeshProUGUI>().text = GetDisplayName(lastBinding.commandName);
            
            //Need a better way to check if it is the mouse
            if (lastBinding.binding == KeyCode.Mouse0 ||
                lastBinding.binding == KeyCode.Mouse1 ||
                lastBinding.binding == KeyCode.Mouse2 ||
                lastBinding.binding == KeyCode.Mouse3 ||
                lastBinding.binding == KeyCode.Mouse4 ||
                lastBinding.binding == KeyCode.Mouse5 ||
                lastBinding.binding == KeyCode.Mouse6)
            {
                lastMouse.GetComponentInChildren<TextMeshProUGUI>().text = GetKeyName(lastBinding.binding);
            }
            else
            {
                lastKey.GetComponentInChildren<TextMeshProUGUI>().text = GetKeyName(lastBinding.binding);
            }
            
            lastKey.GetComponent<TMPButton>().callBack.AddListener(delegate {ChangeKey(lastBinding.commandName);  });
            lastMouse.GetComponent<TMPButton>().callBack.AddListener(delegate { ChangeKey(lastBinding.commandName);});
        }
    }

    private string GetDisplayName(string commandName)
    {
        for (int i = 0; i < keyBindings.Count; i++)
        {
            if (keyBindings[i].commandName == commandName)
            {
                return keyBindings[i].displayName;
            }
        }

        return "Default Key";
    }

    private string GetKeyName(KeyCode binding)
    {
        return binding.ToString();
    }

    public void ChangeKey(string commandName)
    {
        if (!isChangingKey)
        {
            keyChanging = commandName;
            isChangingKey = true; 
        }   
    }


    private void OnGUI()
    {
        if (isChangingKey)
        {
            if (!pak.activeInHierarchy)
                pak.SetActive(true);
            if (Event.current.type == EventType.KeyDown)
            {
                KeyCode lastKey = Event.current.keyCode;

                for (int i = 0; i < keyBindings.Count; i++)
                {
                    if (keyBindings[i].commandName == keyChanging)
                    {
                        keyBindings[i].binding = lastKey;
                        UpdateKey(i);
                        break;
                    }
                }
                isChangingKey = false;
                keyChanging = "";
                pak.SetActive(false);

            }
        }
    }

    private void UpdateKey(int pos)
    {
        Binding lastBinding = keyBindings[pos];
        GameObject lastAction = actionHolder.GetChild(pos).gameObject;
        GameObject lastKey = keyHolder.GetChild(pos).gameObject;
        GameObject lastMouse = mouseHolder.GetChild(pos).gameObject;

        lastAction.GetComponent<TextMeshProUGUI>().text = GetDisplayName(lastBinding.commandName);
        //Need a better way to check if it is the mouse
        if (lastBinding.binding == KeyCode.Mouse0 ||
            lastBinding.binding == KeyCode.Mouse1 ||
            lastBinding.binding == KeyCode.Mouse2 ||
            lastBinding.binding == KeyCode.Mouse3 ||
            lastBinding.binding == KeyCode.Mouse4 ||
            lastBinding.binding == KeyCode.Mouse5 ||
            lastBinding.binding == KeyCode.Mouse6)
        {
            lastMouse.GetComponentInChildren<TextMeshProUGUI>().text = GetKeyName(lastBinding.binding);
        }
        else
        {
            lastKey.GetComponentInChildren<TextMeshProUGUI>().text = GetKeyName(lastBinding.binding);
        }
    }

    private void OnDisable()
    {
        SaveControls();
    }

    public void SaveControls()
    {
        FileManager.SavePlayerConfig(keyBindings);
    }
}

[System.Serializable]
public class Binding
{
    public string displayName;
    public string commandName;
    public EventTrigger.TriggerEvent action; //Unsure about this one at the moment
    public KeyCode binding;

    public Binding(string displayName, string commandName, KeyCode binding)
    {
        this.displayName = displayName;
        this.commandName = commandName;
        this.binding = binding;
    }
}

