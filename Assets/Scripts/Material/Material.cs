

using System.Collections.Generic;

public class Material
{
    public static Registry<Material> REGISTRY = new Registry<Material>();

    public string RegistryId;

    // Runtime Id for e.g. Rendering System.
    public int Id;

    public bool Opaque = true;

    public Material(string id)
    {
        this.RegistryId = id;

        REGISTRY.Register(id, this);
    }

    static Material()
    {

        Material.REGISTRY.BuildNumberIds((mtl, id) => mtl.Id = id);
        Log.info("Material NumId Built.");

        Log.info($"Registered {REGISTRY.Count} Materials: " + REGISTRY.DbgPrintEntries());
    }




    public static Material AIR = null;

    public static Material GRASS = new Material("grass");
    public static Material DIRT = new Material("dirt");
    public static Material DIRT_BLACK = new Material("dirt_black");
    public static Material SAND = new Material("sand");
    public static Material MEADOW = new Material("meadow");
    public static Material MOSS = new Material("moss");
    public static Material FARMLAND = new Material("farmland");

    public static Material STONE = new Material("stone");
    public static Material STONE_BRICK = new Material("stone_brick");
    public static Material TUFF = new Material("tuff");
    public static Material ROCK = new Material("rock");
    public static Material ROCK_JUNGLE = new Material("rock_jungle");
    public static Material ROCK_MOSSY = new Material("rock_mossy");
    public static Material ROCK_SMOOTH = new Material("rock_smooth");
    public static Material ROCK_VOLCANIC = new Material("rock_volcanic");
    public static Material CONCRETE = new Material("concrete");
    public static Material DEEPSLATE = new Material("deepslate");
    public static Material BRICK_ROOF = new Material("brick_roof");

    public static Material IRON = new Material("iron");
    public static Material IRON_TREADPLATE = new Material("iron_treadplate");

    public static Material LOG_OAK = new Material("log_oak");
    public static Material PLANK = new Material("plank");

    public static Material WATER = new Material("water") { Opaque = false };



}