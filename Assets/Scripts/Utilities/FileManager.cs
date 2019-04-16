using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;

public static class FileManager
{
    //Just a standard I want to try and keep
    //At the start of each path should be a \ to indicate a subfolder/file
    //This means at the end of each path it should be just the last folder/file no "\"
    
    //This is when checking for folders
    private enum Folder
    {
        Config
    }
    //This is all the files to check
    private enum Item
    {
        PlayerConfig
    }
    //File extension 
    private const string fileExtension = @".HD"; //idk I just like my own file extension :D (HD = Hidden Detail)
    
    //Config Folder
    //I would like these configs to be similar to source games where each line is just a command for the console
    private const string folderConfig = @"\Configs";
    //Player Config - This will store the players controls
    private const string filePlayerConfig = @"\Player Config" + fileExtension;


    public static List<Binding> GetPlayersConfigAsBindings()
    {
        string[] eachBind = GetPlayersConfig();
        List<Binding> bindings = new List<Binding>();

        for (int i = 0; i < eachBind.Length; i++)
        {
            string[] splitLine = eachBind[i].Split(' ');
            //There seems to be one empty line at the end of the file causing an error
            if (splitLine.Length == 1)
                continue;
            
            if (splitLine[0] == "bind")
            {
                Binding lastBind = new Binding(
                    GetDisplayName(splitLine[2]),
                    splitLine[2],
                    GetKeyCodeFromString(splitLine[1]));

                bindings.Add(lastBind);
            }
        }

        return bindings;
    }

    private static KeyCode GetKeyCodeFromString(string key)
    {
        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (keyCode.ToString() == key)
            {
                return keyCode;
            }
        }

        return KeyCode.None;
    }

    private static string GetDisplayName(string commandName)
    {
        //Not the best of ways as its all hard coded but I'm not sure of another way as it's static
        switch (commandName)
        {
            case "jump":
                return "Jump";
            case "forward":
                return "Forward";
            case "back":
                return "Back";
            case "left":
                return "Left";
            case "right":
                return "Right";
            case "sprint":
                return "Sprint";
            case "inventory":
                return "Inventory";
            case "pause":
                return "Pause";
        }

        return "";
    }

    public static List<Setting> GetPlayersConfigAsSettings()
    {
        string[] eachSetting = GetPlayersConfig();
        List<Setting> settings = new List<Setting>();

        foreach (string line in eachSetting)
        {
            string[] splitLine = line.Split(' ');
            //There seems to be one empty line at the end of the file causing an error
            if (splitLine.Length == 1)
                continue;
            
            if (splitLine[0] == "setting")
            {
                Setting lastSetting = new Setting(
                    splitLine[1],
                    splitLine[2]);
                settings.Add(lastSetting);
            }
            
            
        }
        return settings;
    }

    public static string GetSettingFromConfig(string settingName)
    {
        string value = String.Empty;

        string[] eachSetting = GetPlayersConfig();
        
        foreach (string line in eachSetting)
        {
            string[] splitLine = line.Split(' ');
            if (splitLine[0] == "setting")
            {
                if (splitLine.Length < 4)
                {
                    CustomDebug.Log(CustomDebug.Type.Error,string.Format("Setting: {0} is missing values \n Length = {1}", settingName, splitLine.Length));  
                }
                else if (splitLine[1] == settingName)
                {
                    value = splitLine[2];
                }
            }
        }
        return value;
    }

    public static int GetIntValueFromConfig(string settingName)
    {
        int value;
        string sValue = GetSettingFromConfig(settingName);
        if (!int.TryParse(sValue, out value))
        {
            value = -1;
            CustomDebug.Log(CustomDebug.Type.Error, string.Format("Could not convert {0}'s value of {1} to a int", settingName, sValue));
        }
        
        return value; //This will return -1 if it fails
    }
    
    //This returns an array of each line in that config file
    public static string[] GetPlayersConfig()
    {
        if (CheckFile(Item.PlayerConfig))
        {
            string file = File.ReadAllText(GetCurrentDirectory() + folderConfig + filePlayerConfig);
            return file.Split('\n');
        }
        else
        {
            CustomDebug.Log(CustomDebug.Type.Warning,"File for players config doesn't exist");
            return new string[0];
        }
    }

    private static bool isSaving;
    public static void SavePlayerConfig(List<Binding> bindings,List<Setting> settings)
    {
        if (isSaving)
            return;
        isSaving = true;
        if (CheckFolder(Folder.Config))
        {
            string file = String.Empty;
            for (int i = 0; i < bindings.Count; i++)
            {
                file += string.Format("bind {1} {0} \n", bindings[i].commandName, bindings[i].binding.ToString());
            }

            for (int i = 0; i < settings.Count; i++)
            {
                file += string.Format("setting {0} {1} \n", settings[i].settingName, settings[i].value);
            }
            CustomDebug.Log(CustomDebug.Type.Log, "Saving to players config");
            File.WriteAllText(GetCurrentDirectory() + folderConfig + filePlayerConfig,file);
            isSaving = false;
        }
    }
    

    private static bool CheckFolder(Folder folder)
    {
        string pathToCheck = GetCurrentDirectory();
        switch (folder)
        {
            case Folder.Config:
                pathToCheck += folderConfig;
                break;
            default:
                Debug.LogError("Not found that folder " + folder.ToString());
                break;
        }

        if (Directory.Exists(pathToCheck))
        {
            return true;
        }
        else
        {
            //If it doesn't exist, lets try and create it
            try
            {
                //I am not sure as I can't find anything about .exists in docs but hopefully it returns true
                return Directory.CreateDirectory(pathToCheck).Exists;
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Failed to create folder: {0}", pathToCheck));
                return false;
            }
        }
    }

    private static bool CheckFile(Item file)
    {
        string pathToCheck = GetCurrentDirectory();
        switch (file)
        {
            case Item.PlayerConfig:
                if (CheckFolder(Folder.Config))
                {
                    return File.Exists(pathToCheck + folderConfig + filePlayerConfig);
                }
                else
                {
                    return false; 
                }
        }

        return false;
    }

    //Made this method just in case I need to change it, saves me changing it every where
    private static string GetCurrentDirectory()
    {
        return Directory.GetCurrentDirectory();
    }
}


/*

Player Config.HD

# bind KeyCode action
When a line start with "bind" that means its setting a button up for the players to use in game
This get split up into two other variables
## Keycode
Keycode is the key which is used for that action, the format for this has to match the output of Keycode.ToString()
## action
Action is set by the game and is used to find the action you want to do when pressing that button such as jump

# setting settingName value
When a line starts with "setting" this is a graphics setting to the client
## settingName 
SettingName will be the name of which setting it is changing
i_ means that this should be a hole number
f_ means that this can be a float
b_ means that this is true or false
s_ means that this is a string
## value 
Value is a string of what it could possiably be

*/

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

[Serializable]
public class Setting
{
    public string settingName;
    public string value;

    public Setting(string settingName, string value)
    {
        this.settingName = settingName;
        this.value = value;
    }
}