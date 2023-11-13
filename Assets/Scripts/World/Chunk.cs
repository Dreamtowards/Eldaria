using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Ethertia
{
    public class Chunk : MonoBehaviour
    {
        public Cell[,,] m_Cells = new Cell[16, 16, 16];

        // A reference to the World
        public World m_World;

        public float3 chunkpos;

        public bool m_Dirty = true;

        void Start()
        {
        }


        void Update()
        {

        }


        //public void UpdateMesh()
        //{
        //    VertexData vtx = new VertexData();

        //    UnityEngine.Profiling.Profiler.BeginSample("[Et] MeshGen");

        //    // Generate Mesh
        //    ChunkMesher.GenerateMesh(this, vtx);

        //    Mesh mesh = new Mesh();

        //    vtx.Export(mesh);
        //    mesh.triangles = Maths.Sequence(vtx.VertexCount());

        //    //mesh.RecalculateNormals();

        //    UnityEngine.Profiling.Profiler.EndSample();

        //    Debug.Log("Chunk " + chunkpos + " Mesh Generated, VertexCount: " + vtx.VertexCount());

        //    UpdateMesh(mesh);
        //}

        public void UpdateMesh(Mesh mesh)
        {

            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }



        // Get Cell at Chunk Local.
        public ref Cell LocalCell(int rx, int ry, int rz)
        {
            return ref m_Cells[rx, ry, rz];
        }
        public ref Cell LocalCell(float3 rpos, bool worldwide = false)
        {
            if (worldwide && !InBound(rpos))
            {
                return ref m_World.GetCell(chunkpos + rpos);
            }

            return ref LocalCell((int)rpos.x, (int)rpos.y, (int)rpos.z);
        }



        public static bool InBound(float3 rpos)
        {
            return rpos.x >= 0 && rpos.x < 16 &&
                   rpos.y >= 0 && rpos.y < 16 &&
                   rpos.z >= 0 && rpos.z < 16;
        }
        public static float3 ChunkPos(float3 p)
        {
            return new(Maths.Floor(p.x, 16), Maths.Floor(p.y, 16), Maths.Floor(p.z, 16));
        }
        public static float3 LocalPos(float3 p)
        {
            return new(Maths.Mod(p.x, 16), Maths.Mod(p.y, 16), Maths.Mod(p.z, 16));
        }
        //public static float3 ChunkPos(Vector3 p)
        //{
        //    return new float3(Maths.Floor(p.x, 16), Maths.Floor(p.y, 16), Maths.Floor(p.z, 16));
        //}
        //public static float3 LocalPos(Vector3 p)
        //{
        //    return new float3(Maths.Mod(p.x, 16), Maths.Mod(p.y, 16), Maths.Mod(p.z, 16));
        //}


    }
}