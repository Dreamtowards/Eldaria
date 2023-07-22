using System;


public class ChunkGenerator
{

    public static void GenerateChunk(Chunk chunk)
    {
        chunk.LocalCell(0, 0, 0).BlockId = 10;

    }

}
