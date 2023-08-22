using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


class Ethertia
{
    //public static Dictionary<int, int> m_SomeData = new Dictionary<int, int>();

    //[RuntimeInitializeOnLoadMethod]
    //static void Init()
    //{
    //    Debug.Log("After Scene is loaded and game is running");

    //}

    //static Ethertia()
    //{
    //    Log.info("Ethertia StaticNew(); "+m_SomeData.Count); 
    //}

    public static World GetWorld()
    {
        return World._Inst;
    }
}

class EthertiaTools
{

    [MenuItem("Eldaria/Regenerate Material Texture Atlases")]
    private static void RegenMtlTexAtlas()
    {
        //Log.info($"CurrDir: {Directory.GetCurrentDirectory()}, {Application.dataPath}");

        MaterialTextures.Load();

        //EditorUtility.DisplayDialog("MyTool2", "Do It in C# !", "OK", "");
    }

}