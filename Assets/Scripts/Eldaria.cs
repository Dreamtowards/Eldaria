using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;


class EthertiaTools
{

    [MenuItem("Eldaria/Regenerate Material Texture Atlases")]
    private static void RegenMtlTexAtlas()
    {
        Log.info($"CurrDir: {Directory.GetCurrentDirectory()}, {Application.absoluteURL}");

        MaterialTextures.Load();

        //EditorUtility.DisplayDialog("MyTool2", "Do It in C# !", "OK", "");
    }

}