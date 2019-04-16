using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Settings")]
    public List<Setting> settings = new List<Setting>();

    [SerializeField] private DefaultSettings defaultSettings;
    [SerializeField] private ControlsUI controlsUi;

    [Header("Game Objects")] 
    [SerializeField] private TMP_InputField viewDistance;

    private void Start()
    {
        LoadPlayersConfig();
    }

    public void LoadDefaultSettings()
    {
        settings.Clear();
        for (int i = 0; i < defaultSettings.defaultSettings.Count; i++)
        {
            Setting lastSetting = new Setting(
                defaultSettings.defaultSettings[i].settingName,
                defaultSettings.defaultSettings[i].value);
            settings.Add(lastSetting);
        }
        ReloadUI();
    }

    public void LoadPlayersConfig()
    {
        settings.Clear();
        List<Setting> playerSettings = FileManager.GetPlayersConfigAsSettings();
        //This is to check if the players config wasn't already created
        if (playerSettings.Count == 0)
        {
            CustomDebug.Log(CustomDebug.Type.Log,"Loading Default Settings");
            LoadDefaultSettings();
            return;
        }
        foreach (Setting setting in playerSettings)
        {
            settings.Add(setting);
        }
        ReloadUI();
    }

    private void ReloadUI()
    {
        foreach (Setting setting in settings)
        {
            switch (setting.settingName)
            {
                case "i_viewdistance":
                    viewDistance.text = setting.value;
                    break;
                default:
                    CustomDebug.Log(CustomDebug.Type.Warning,string.Format("Couldn't find the setting called {0} with the value of {1} ", setting.settingName, setting.value));
                    break;
            }
        }
    }
    
    public void SaveGraphics()
    {
        FileManager.SavePlayerConfig(controlsUi.keyBindings,settings);
    }

    public void UpdateSetting(TMP_InputField inputField)
    {
        for (int i = 0; i < settings.Count; i++)
        {
            if (settings[i].settingName == inputField.name)
            {
                settings[i].value = inputField.text;
                return;
            }
        }
    }

    private void OnDisable()
    {
        SaveGraphics();
    }
}


