using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ChunkMesher
{

    public static void GenerateMesh(Chunk chunk, VertexData vts)
    {
        SN_GenerateMesh(chunk, vts);

        //for (int rx = 0; rx < 16; ++rx)
        //{
        //    for (int ry = 0; ry < 16; ++ry)
        //    {
        //        for (int rz = 0; rz < 16; ++rz)
        //        {
        //            Vector3 rpos = new Vector3(rx, ry, rz);
        //            Cell cell = chunk.LocalCell(rx, ry, rz);

        //            if (cell.IsSolid())
        //            {
        //                PutCube(vts, rpos, chunk);
        //            }
        //        }
        //    }
        //}
    }









    ////////////////// Blocky Cube //////////////////

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
            if (!neibCell.IsSolid())  // !isOpaque
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










    ////////////////// Surface Nets, Isosurface //////////////////


    static Vector3[] VERT = {
            new(0, 0, 0),  // 0
            new(0, 0, 1),
            new(0, 1, 0),  // 2
            new(0, 1, 1),
            new(1, 0, 0),  // 4
            new(1, 0, 1),
            new(1, 1, 0),  // 6
            new(1, 1, 1)
    };
    // from min to max in each Edge.  axis order x y z.
    // Diagonal Edge in Cell is in-axis-flip-index edge.  i.e. diag of edge[axis*4 +i] is edge[axis*4 +(3-i)]
    /*     +--2--+    +-----+    +-----+
     *    /|    /|   /7    /6  11|    10
     *   +--3--+ |  +-----+ |  +-----+ |
     *   | +--0|-+  5 +---4-+  | +---|-+
     *   |/    |/   |/    |/   |9    |8
     *   +--1--+    +-----+    +-----+
     *   |3  2| winding. for each axis.
     *   |1  0|
     */
    static int[,] EDGE = {  // [12,2]
            {0,4}, {1,5}, {2,6}, {3,7},  // X
            {5,7}, {1,3}, {4,6}, {0,2},  // Y
            {4,5}, {0,1}, {6,7}, {2,3}   // Z
    };
    static Vector3[] AXES = {
            new(1, 0, 0),
            new(0, 1, 0),
            new(0, 0, 1)
    };
    static Vector3[,] ADJACENT = {
            {new(0,0,0), new(0,-1,0), new(0,-1,-1), new(0,-1,-1), new(0,0,-1), new(0,0,0)},
            {new(0,0,0), new(0,0,-1), new(-1,0,-1), new(-1,0,-1), new(-1,0,0), new(0,0,0)},
            {new(0,0,0), new(-1,0,0), new(-1,-1,0), new(-1,-1,0), new(0,-1,0), new(0,0,0)}
    };

    static void SN_GenerateMesh(Chunk chunk, VertexData vts)
    {

        for (int rx = 0; rx < 16; ++rx)
        {
            for (int ry = 0; ry < 16; ++ry)
            {
                for (int rz = 0; rz < 16; ++rz)
                {
                    Vector3 rp = new Vector3(rx, ry, rz);
                    ref Cell c0 = ref chunk.LocalCell(rx, ry, rz);

                    // for 3 axes edges, if sign-changed, connect adjacent 4 cells' vertices
                    for (int axis_i = 0; axis_i < 3; ++axis_i)
                    {
                        ref Cell c1 = ref chunk.LocalCell(rp + AXES[axis_i], true);

                        if (SN_SignChanged(c0, c1))
                        {
                            for (int quadv_i = 0; quadv_i < 6; ++quadv_i)
                            {
                                int winded_vi = c0.IsSolid() ? quadv_i : 5 - quadv_i;
                                Vector3 quadp = rp + ADJACENT[axis_i, winded_vi];

                                ref Cell c = ref chunk.LocalCell(quadp, true);

                                //if (c.FeaturePoint.x == Mathf.Infinity)
                                {
                                    c.FeaturePoint = SN_FeaturePoint(quadp, chunk);
                                    c.Normal = SN_Grad(quadp, chunk);
                                }

                                Vector3 p = quadp + c.FeaturePoint;

                                // Select Material of 8 Corners. (Max Density Value)
                                int MtlId = 1;

                                vts.AddVertex(p, new(MtlId, -1), -c.Normal);
                            }
                        }
                    }
                }
            }
        }
    }

    static bool SN_SignChanged(in Cell c0, in Cell c1)
    {
        return (c0.Value > 0) != (c1.Value > 0);
    }

    // Evaluate Normal of a Cell FeaturePoint
    // via Approxiate Differental Gradient  
    static Vector3 SN_Grad(Vector3 rp, Chunk chunk)
    {
        float E = 1.0f;  // epsilon
        float denom = 1.0f / (2.0f * E);
        return Vector3.Normalize(denom * new Vector3(
            chunk.LocalCell(rp + new Vector3(E,0,0), true).Value - chunk.LocalCell(rp - new Vector3(E,0,0), true).Value,
            chunk.LocalCell(rp + new Vector3(0,E,0), true).Value - chunk.LocalCell(rp - new Vector3(0,E,0), true).Value,
            chunk.LocalCell(rp + new Vector3(0,0,E), true).Value - chunk.LocalCell(rp - new Vector3(0,0,E), true).Value));
    }

    // Evaluate FeaturePoint
    // returns cell-local point.
    static Vector3 SN_FeaturePoint(Vector3 rp, Chunk chunk)
    {
        int signchanges = 0;
        Vector3 fp_sum = new Vector3(0,0,0);

        for (int edge_i = 0; edge_i < 12; ++edge_i)
        {
            Vector3 v0 = VERT[EDGE[edge_i, 0]];
            Vector3 v1 = VERT[EDGE[edge_i, 1]];
            ref Cell c0 = ref chunk.LocalCell(rp + v0, true);
            ref Cell c1 = ref chunk.LocalCell(rp + v1, true);

            if (SN_SignChanged(c0, c1))
            {
                float t = Mathf.InverseLerp(c0.Value, c1.Value, 0);
                Vector3 p = t * (v1-v0) + v0;

                fp_sum += p;
                ++signchanges;
            }
        }
        Assert.IsTrue(signchanges != 0, "Invalid FeaturePoint, No SignChange.");
        return fp_sum / signchanges;
    }
}

