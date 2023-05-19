using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class DebugHelper
{
    private static readonly StringBuilder logBuilder = new StringBuilder();

    public static void Log(string message)
    {
        logBuilder.Append(message);
        Debug.Log(logBuilder.ToString());
        logBuilder.Length = 0;
    }

    public static void LogFormat(string format, params object[] args)
    {
        logBuilder.AppendFormat(format, args);
        Debug.Log(logBuilder.ToString());
        logBuilder.Length = 0;
    }

    public static void LogError(string errorMessage)
    {
        logBuilder.Append(errorMessage);
        Debug.LogError(logBuilder.ToString());
        logBuilder.Length = 0;
    }

    public static void LogFormatError(string format, params object[] args)
    {
        logBuilder.AppendFormat(format, args);
        Debug.LogError(logBuilder.ToString());
        logBuilder.Length = 0;
    }

    public static void LogWarning(string warningMessage)
    {
        logBuilder.Append(warningMessage);
        Debug.LogWarning(logBuilder.ToString());
        logBuilder.Length = 0;
    }

    public static void LogFormatWarning(string format, params object[] args)
    {
        logBuilder.AppendFormat(format, args);
        Debug.LogWarning(logBuilder.ToString());
        logBuilder.Length = 0;
    }
}
