using System;
using UnityEngine;

public class ChunkMesher
{

    public static void GenerateChunkMesh(Chunk chunk, Mesh mesh)
    {
        VertexData vdat = new VertexData();

        for (int rx = 0; rx < 16; ++rx)
        {
            for (int ry = 0; ry < 16; ++ry)
            {
                for (int rz = 0; rz < 16; ++rz)
                {
                    Vector3 rpos = new Vector3(rx, ry, rz);
                    Cell cell = chunk.LocalCell(rx, ry, rz);

                    if (cell.BlockId != 0)
                    {
                        PutCube(vdat, rpos, chunk);
                    }
                }
            }
        }
        //Vector3[] vts = {
        //    new Vector3(0, 0, 0),
        //    new Vector3(0, 0, 1),
        //    new Vector3(1, 0, 0)
        //};
        //int[] idx = { 0, 1, 2 };

        //mesh.vertices = vts;

        vdat.Export(mesh);
        
        mesh.triangles = Maths.Sequence(vdat.VertexCount());

        mesh.RecalculateNormals();

        Debug.Log("Chunk " + chunk.Position() + " Mesh Generated, VertexCount: " + vdat.VertexCount());
    }


    static float[] CUBE_POS = {
            0, 0, 1, 0, 1, 1, 0, 1, 0,  // Left -X
            0, 0, 1, 0, 1, 0, 0, 0, 0,
            1, 0, 0, 1, 1, 0, 1, 1, 1,  // Right +X
            1, 0, 0, 1, 1, 1, 1, 0, 1,
            0, 0, 1, 0, 0, 0, 1, 0, 0,  // Bottom -Y
            0, 0, 1, 1, 0, 0, 1, 0, 1,
            0, 1, 1, 1, 1, 1, 1, 1, 0,  // Bottom +Y
            0, 1, 1, 1, 1, 0, 0, 1, 0,
            0, 0, 0, 0, 1, 0, 1, 1, 0,  // Front -Z
            0, 0, 0, 1, 1, 0, 1, 0, 0,
            1, 0, 1, 1, 1, 1, 0, 1, 1,  // Back +Z
            1, 0, 1, 0, 1, 1, 0, 0, 1,
    };
    static float[] CUBE_UV = {
            1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0,  // One Face.
            1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0,
            1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0,
            1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0,
            1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0,
            1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0,
    }; 
    static float[] CUBE_NORM = {
            -1, 0, 0,-1, 0, 0,-1, 0, 0,-1, 0, 0,-1, 0, 0,-1, 0, 0,
            1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0,
            0,-1, 0, 0,-1, 0, 0,-1, 0, 0,-1, 0, 0,-1, 0, 0,-1, 0,
            0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0,
            0, 0,-1, 0, 0,-1, 0, 0,-1, 0, 0,-1, 0, 0,-1, 0, 0,-1,
            0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1
    };
    static void PutCube(VertexData vdat, Vector3 rpos, Chunk chunk) 
    {
        for (int face_i = 0; face_i < 6; ++face_i)
        {
            Vector3 face_dir = Maths.Vector3(CUBE_NORM, face_i * 18);   // 18: 3 scalar * 3 vertex * 2 triangle

            Cell neibCell = chunk.LocalCell(rpos + face_dir, true);
            if (neibCell.BlockId == 0)  // !isOpaque
            {
                for (int vert_i = 0; vert_i < 6; ++vert_i)
                {
                    vdat.AddVertex(Maths.Vector3(CUBE_POS, face_i * 18 + vert_i * 3) + rpos,
                                   Maths.Vector2(CUBE_UV,  face_i * 12 + vert_i * 2),
                                   Maths.Vector3(CUBE_NORM,face_i * 18 + vert_i * 3));
                }
            }
        }
    }
}
