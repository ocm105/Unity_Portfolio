using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowDebug : Debug
{
    public static void SuccessLog(string message)
    {
#if UNITY_EDITOR_WIN
        unityLogger.Log(LogType.Log, $"<color=green>{message}</color>");
#endif
    }

    public static void FailLog(string message)
    {
#if UNITY_EDITOR_WIN
        unityLogger.Log(LogType.Log, $"<color=red>{message}</color>");
#endif
    }
}
