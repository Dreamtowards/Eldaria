using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Cell[,,] m_Cells = new Cell[16, 16, 16];

    // A reference to the World
    public World m_World;

    public bool m_Dirty = true;

    void Start()
    {

    }


    void Update()
    {

    }


    public void UpdateMesh()
    {
        Mesh mesh = new Mesh();

        ChunkMesher.GenerateChunkMesh(this, mesh);

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public float3 Position()
    {
        return transform.position;
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
            return ref m_World.GetCell(Position() + rpos);
        }

        return ref LocalCell((int)rpos.x, (int)rpos.y, (int)rpos.z);
    }



    public static bool InBound(Vector3 rpos)
    {
        return rpos.x >= 0 && rpos.x < 16 &&
               rpos.y >= 0 && rpos.y < 16 &&
               rpos.z >= 0 && rpos.z < 16;
    }
    public static int3 ChunkPos(float3 p)
    {
        return new int3((int)Maths.Floor(p.x, 16), (int)Maths.Floor(p.y, 16), (int)Maths.Floor(p.z, 16));
    }
    public static int3 LocalPos(float3 p)
    {
        return new int3((int)Maths.Mod(p.x, 16), (int)Maths.Mod(p.y, 16), (int)Maths.Mod(p.z, 16));
    }


}
