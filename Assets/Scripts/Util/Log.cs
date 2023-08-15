

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
}