

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
            throw new System.Exception(msg);
        }
    }
}