

//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
//using System.IO;


using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

class MaterialTextures
{
    public static int TEX_RESOLUTION = 512;

    public static void Load()
    {
        using BenchmarkTimer tm = new();
        Log.info($"Loading {Material.REGISTRY.Count} material textures/atlases... (x{TEX_RESOLUTION})");

        MakeAtlas("diff", TEX_RESOLUTION, "cache_atlas_diff.png");
        MakeAtlas("norm", TEX_RESOLUTION, "cache_atlas_norm.png");
        MakeAtlas("dram", TEX_RESOLUTION, "cache_atlas_dram.png", true);

        Log.info("Material textures atlases all loaded/generated.");
    }

    //public static bool LoadTex_ResizeCopy(string mtlId, string texType, Graphics atlas, int x, int size, int onlyChannelARGB = -1)
    //{
    //    RenderTexture tmp = RenderTexture.GetTemporary(size, size, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
    //    RenderTexture.active = tmp;
    //    Graphics.Blit(texType, tmp);



    //    string path = $"material/{mtlId}/{texType}.png";
    //    if (File.Exists(path))
    //    {
    //        Texture2D src = new Texture2D(0,0);
    //        src.LoadImage(File.ReadAllBytes(path));


    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    private static bool LoadResizePut(string id, string texType, Texture2D dstTex, int dstWidth, int dstHeight, int dstX = 0)
    {
        string path = $"{Application.dataPath}/assets/materials/{id}/{texType}.png";
        if (!File.Exists(path))
            return false;

        Texture2D src = new Texture2D(0, 0);
        src.LoadImage(File.ReadAllBytes(path));
        ResizePut(src, dstTex, dstWidth, dstHeight, dstX);
        return true;
    }

    // dont forget dstTex.Apply();
    private static void ResizePut(Texture2D srcTex, Texture2D dstTex, int dstWidth, int dstHeight, int dstX)
    {
        srcTex.filterMode = FilterMode.Bilinear;  // before Graphics.Blit()

        RenderTexture tmp = RenderTexture.GetTemporary(dstWidth, dstHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        //RenderTexture.active = tmp;
        Graphics.Blit(srcTex, tmp);
        //texture.Resize(newWidth, newHeight, texture.format, false);

        dstTex.ReadPixels(new Rect(0, 0, dstWidth, dstHeight), dstX, 0);
        dstTex.Apply();  // Upload to GPU?

        RenderTexture.ReleaseTemporary(tmp);
    }

    private static void CopyChannel(Texture2D srcTex, Texture2D dstTex, int srcChannel, int dstChannel)
    {
        Color32[] pSrc = srcTex.GetPixels32();
        Color32[] pDst = dstTex.GetPixels32();
        Assert.IsTrue(pSrc.Length == pDst.Length);

        for (int i = 0; i < pSrc.Length; ++i)
        {
            pDst[i][dstChannel] = pSrc[i][srcChannel];
        }
        dstTex.SetPixels32(pDst);
        dstTex.Apply();  // could move to outer for Optimizer Performance.
    }

    public static void MakeAtlas(string texType, int PX, string cacheFile, bool DRAM = false)
    {
        using BenchmarkTimer tm = new();
        Log.info($" *{texType} generate new atlas to '{cacheFile}'.");

        int N = Material.REGISTRY.Count;

        Texture2D atlas = new Texture2D(N*PX, PX, TextureFormat.ARGB32, false);

        if (DRAM)
        {
            Texture2D original = new Texture2D(PX, PX);
            Texture2D composed = new Texture2D(PX, PX);

            int i = 0;
            foreach (var it in Material.REGISTRY)
            {
                if (LoadResizePut(it.Key, "disp", original, PX, PX))
                    CopyChannel(original, composed, 0, 0);

                if (LoadResizePut(it.Key, "rough", original, PX, PX))
                    CopyChannel(original, composed, 0, 1);

                if (LoadResizePut(it.Key, "ao", original, PX, PX))
                    CopyChannel(original, composed, 0, 2);

                if (LoadResizePut(it.Key, "metal", original, PX, PX))
                    CopyChannel(original, composed, 0, 3);

                // actually there do not need Resize, just copy.
                ResizePut(composed, atlas, PX, PX, i * PX);

                ++i;
            }
        }
        else
        {
            Texture2D src = new Texture2D(0, 0);

            int i = 0;
            foreach (var it in Material.REGISTRY)
            {
                LoadResizePut(it.Key, texType, atlas, PX, PX, i * PX);

                ++i;
            }
        }

        File.WriteAllBytes(cacheFile, atlas.EncodeToPNG());
    }
}