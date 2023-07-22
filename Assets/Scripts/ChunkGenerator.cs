using System;
using UnityEngine;

public class ChunkGenerator
{

    public static void GenerateChunk(Chunk chunk)
    {

        for (int rx = 0; rx < 16; ++rx)
        {
            for (int ry = 0; ry < 16; ++ry)
            {
                for (int rz = 0; rz < 16; ++rz)
                {
                    Vector3 p = new Vector3(rx, ry, rz) + chunk.Position();
                    ref Cell cell = ref chunk.LocalCell(rx, ry, rz);

                    float noise = Mathf.PerlinNoise(p.x / 10f, p.z/10f);
                    if (ry/16.0f < noise)
                    {
                        cell.BlockId = 10;
                    }
                }
            }
        }
    }

}
