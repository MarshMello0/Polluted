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
        UnityEngine.Debug.unityLogger.Log(LogType.Log,finalMessage);
    }
}
