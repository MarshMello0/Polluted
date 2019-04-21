using System;
using UnityEngine;

public static class CustomDebug
{
    public enum Type
    {
        Log,Warning,Error
    }

    public static void Log(Type type, string message)
    {
        string finalMessage = String.Empty;
        switch (type)
        {
            case Type.Log:
                finalMessage += "<color=#000000ff>";
                break;
            case Type.Warning:
                finalMessage += "<color=#ffff00ff>";
                break;
            case Type.Error:
                finalMessage += "<color=#ff0000ff>";
                break;
        }

        finalMessage += message;
        finalMessage += " </color>";
        Debug.Log(finalMessage);
    }

    public static void LogColour(Color colour, string message)
    {
        string finalMessage = "<color=#" + ColorUtility.ToHtmlStringRGB(colour) + ">";
        finalMessage += message + "</color>";
        
        Debug.Log(finalMessage);
    }
}
