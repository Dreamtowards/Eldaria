using System;
using UnityEngine;

public class ChunkGenerator
{

    public static void GenerateChunk(Chunk chunk)
    {
        FastNoiseLite fn = new FastNoiseLite();
        fn.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        for (int rx = 0; rx < 16; ++rx)
        {
            for (int ry = 0; ry < 16; ++ry)
            {
                for (int rz = 0; rz < 16; ++rz)
                {
                    Vector3 p = new Vector3(rx, ry, rz) + chunk.Position();
                    ref Cell cell = ref chunk.LocalCell(rx, ry, rz);


                    float noise = Mathf.PerlinNoise(p.x/30.0f, p.z/30.0f) - p.y/20.0f;//fn.GetNoise(p.x/10f, p.y/10f, p.z/10f);
                    //Debug.Log($"NoiseF: {noise}");

                    if (noise >= 0)//ry/16.0f < noise)
                    {
                        cell.MtlId = 1;
                    }
                    cell.Value = noise;

                }
            }
        }
    }

}
