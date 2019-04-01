using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
            Binding lastBind = new Binding(
                GetDisplayName(splitLine[2]),
                splitLine[2],
                GetKeyCodeFromString(splitLine[1]));
            
            bindings.Add(lastBind);
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
        }

        return "";
    }
    
    //This returns an array of each line in that config file
    public static string[] GetPlayersConfig()
    {
        if (CheckFolder(Folder.Config))
        {
            string file = File.ReadAllText(GetCurrentDirectory() + folderConfig + filePlayerConfig);
            return file.Split('\n');
        }
        else
        {
            Debug.LogError("Folder for players config doesn't exist");
            return null;
        }
    }

    public static void SavePlayerConfig(List<Binding> bindings)
    {
        if (CheckFolder(Folder.Config))
        {
            string file = String.Empty;
            for (int i = 0; i < bindings.Count; i++)
            {
                file += string.Format("bind {1} {0} \n", bindings[i].commandName, bindings[i].binding.ToString());
            }
            File.WriteAllText(GetCurrentDirectory() + folderConfig + filePlayerConfig,file);
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

    //Made this method just in case I need to change it, saves me changing it every where
    private static string GetCurrentDirectory()
    {
        return Directory.GetCurrentDirectory();
    }
}
