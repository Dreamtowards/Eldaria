

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;


class MaterialTextures
{
    public static int TEX_RESOLUTION = 512;

    public static void Load()
    {
        BenchmarkTimer _;
        Log.info($"Loading {Material.REGISTRY.Count()} material textures/atlases... (x{TEX_RESOLUTION})");

        MakeAtlas("diff", TEX_RESOLUTION, "cache/atlas_diff.png");
        MakeAtlas("norm", TEX_RESOLUTION, "cache/atlas_norm.png");
        MakeAtlas("dram", TEX_RESOLUTION, "cache/atlas_dram.png");

        Log.info("Material textures atlases all loaded/generated.");
    }

    public static bool LoadTex_Resize(string mtlId, string texType, Graphics gAtlas, int x, int size, int onlyChannelARGB = -1)
    {
        string path = $"material/{mtlId}/{texType}.png";
        if (File.Exists(path))
        {
            using Image src = Image.FromFile(path);

            //ColorMatrix colmat = new();
            //colmat.Matrix00 = rgbaChannelMask.x;
            //colmat.Matrix11 = rgbaChannelMask.y;
            //colmat.Matrix22 = rgbaChannelMask.z;
            //colmat.Matrix33 = rgbaChannelMask.w;

            ImageAttributes imgAttr = new ImageAttributes();
            //imgAttr.SetColorMatrix(colmat);
            imgAttr.SetOutputChannel((ColorChannelFlag)onlyChannelARGB);

            gAtlas.DrawImage(src, new Rectangle(x, 0, size, size), 0, 0, src.Width, src.Height, GraphicsUnit.Pixel, imgAttr);

            return true;
        }
        else
        {
            return false;
        }
    }

    public static void MakeAtlas(string texType, int PX, string cacheFile, bool DRAM = false)
    {
        BenchmarkTimer tm;
        Log.info($" *{texType} generate new atlas to '{cacheFile}'.");

        int N = Material.REGISTRY.Count();

        using Bitmap atlasImage = new Bitmap(N*PX, PX);

        using Graphics gAtlas = Graphics.FromImage(atlasImage);
        gAtlas.SmoothingMode = SmoothingMode.AntiAlias;
        gAtlas.InterpolationMode = InterpolationMode.HighQualityBilinear;
        gAtlas.PixelOffsetMode = PixelOffsetMode.HighQuality;

        if (DRAM)
        {

        }
        else
        {
            Bitmap resized = new Bitmap(PX, PX); 

            int i = 0;
            foreach (var it in Material.REGISTRY)
            {
                //if (LoadTex_Resize(it.Key, texType, resized))  // Loaded Texture and Resized
                {

                }
                

                ++i;
            }
        }

        atlasImage.Save(cacheFile);
    }
}