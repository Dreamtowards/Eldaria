

using System;

class Log
{
    public static void info(string fmt)
    {
        UnityEngine.Debug.Log(fmt);
    }
    public static void warn(string fmt)
    {
        UnityEngine.Debug.LogWarning(fmt);
    }
    public static void Error(string fmt)
    {
        UnityEngine.Debug.LogError(fmt);
    }

    public static void assert(bool should, string msg = "Assert Error Message")
    {
        if (!should)
        {
            Log.warn(msg);
            //throw new System.Exception(msg);
        }
    }
    public static void assert(bool should, Func<string> str_fn)
    {
        if (!should)
        {
            Log.warn(str_fn());
            //throw new System.Exception(msg);
        }
    }
}