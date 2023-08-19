using System;
using Unity.Mathematics;
using UnityEngine;

public class ChunkGenerator
{

    public static void GenerateChunk(Chunk chunk)
    {
        FastNoiseLite fn = new FastNoiseLite();
        fn.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        fn.SetFrequency(0.15f);
        fn.SetFractalLacunarity(2f);
        fn.SetFractalGain(0.5f);
        fn.SetFractalType(FastNoiseLite.FractalType.FBm);

        for (int rx = 0; rx < 16; ++rx)
        {
            for (int ry = 0; ry < 16; ++ry)
            {
                for (int rz = 0; rz < 16; ++rz)
                {
                    float3 p = new float3(rx, ry, rz) + chunk.chunkpos;
                    ref Cell cell = ref chunk.LocalCell(rx, ry, rz);


                    float noise = fn.GetNoise(p.x / 12f, p.y/12f, p.z/12f);
                    // Mathf.PerlinNoise(p.x/30.0f, p.z/30.0f) - p.y/20.0f;

                    //Debug.Log($"NoiseF: {noise}");

                    cell.Value = noise;
                    //if (noise >= 0)//ry/16.0f < noise)
                    {
                        cell.MtlId = p.y > 4 ? 1 : 2;
                    }

                }
            }
        }
    }

}
