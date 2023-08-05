using System;
using Unity.Mathematics;
using UnityEngine;

public class ChunkGenerator
{

    public static void GenerateChunk(Chunk chunk)
    {
        FastNoiseLite fn = new FastNoiseLite();
        fn.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fn.SetFractalOctaves(4);

        for (int rx = 0; rx < 16; ++rx)
        {
            for (int ry = 0; ry < 16; ++ry)
            {
                for (int rz = 0; rz < 16; ++rz)
                {
                    float3 p = new float3(rx, ry, rz) + chunk.Position();
                    ref Cell cell = ref chunk.LocalCell(rx, ry, rz);

                    float noise = fn.GetNoise(p.x/10f, p.y/10f, p.z/10f);
                    if (noise < -0.1f)//ry/16.0f < noise)
                    {
                        cell.BlockId = 10;
                    }
                }
            }
        }
    }

}
