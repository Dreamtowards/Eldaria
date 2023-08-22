using System;
using Unity.Mathematics;
using UnityEngine;

public class ChunkGenerator
{

    public static void GenerateChunk(Chunk chunk)
    {
        FastNoiseLite fn = new FastNoiseLite();
        fn.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fn.SetFractalType(FastNoiseLite.FractalType.FBm);

        World w = Ethertia.GetWorld();
        fn.SetFractalOctaves((int)w.m_DbgN_Octaves);
        fn.SetFrequency(1/w.m_DbgN_FreqInv);
        fn.SetFractalLacunarity(w.m_DbgN_FracLat);
        fn.SetFractalGain(w.m_DbgN_FracGain);
        fn.SetSeed((int)w.m_Seed);

        for (int rx = 0; rx < 16; ++rx)
        {
            for (int ry = 0; ry < 16; ++ry)
            {
                for (int rz = 0; rz < 16; ++rz)
                {
                    float3 p = new float3(rx, ry, rz) + chunk.chunkpos;


                    float noise3d = fn.GetNoise(p.x, p.y, p.z) + 0.6f;  // 怎么噪声出来全是 -0.4左右的，没有0+的值
                    float noise2d = fn.GetNoise(p.x / 2.0f, p.z / 2.0f) + 0.6f;

                    float v = noise3d - p.y / 90;

                    Material mtl = null;
                    if (v > 0)
                    {
                        mtl = Material.STONE;
                    }
                    else if (p.y < 0)
                    {
                        mtl = Material.WATER;
                    }

                    ref Cell cell = ref chunk.LocalCell(rx, ry, rz);
                    cell.Value = v;
                    cell.Mtl = mtl;
                }
            }
        }

        Populate(chunk);
    }

    // Populate is not for a single chunk, but always effects to neibough chunks
    public static void Populate(Chunk chunk)
    {
        FastNoiseLite fnSand = new FastNoiseLite();
        fnSand.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        for (int rx = 0; rx < 16; ++rx)
        {
            for (int rz = 0; rz < 16; ++rz)
            {
                float x = chunk.chunkpos.x + rx;
                float z = chunk.chunkpos.z + rz;
                float nvSand = fnSand.GetNoise(x, z);

                int deep = int.MaxValue;  // Y 轴离上方表面的"深度"，用于判断表面材料层的填充

                for (int ry = 15; ry >= 0; --ry)
                {
                    float y = chunk.chunkpos.y + ry;
                    ref Cell c = ref chunk.LocalCell(rx, ry, rz);
                    Material mtl = c.Mtl;

                    // skip. AIR or WATER.
                    if (mtl != Material.STONE)
                    {
                        deep = 0;
                        continue;
                    }

                    if (deep <= 3 && y < 3 && nvSand > 0.3f)
                    {
                        mtl = Material.SAND;
                    }
                    else
                    {
                        if (deep == 0)
                        {
                            mtl = Material.MOSS;
                        }
                        else if (deep <= 4)
                        {
                            mtl = Material.DIRT;
                        }
                    }

                    c.Mtl = mtl;
                    if (deep != int.MaxValue)
                        ++deep;
                }
            }
        }
    }
}
