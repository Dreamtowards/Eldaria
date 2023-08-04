using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Unity.Mathematics;

public class World : MonoBehaviour
{
    Dictionary<Vector3, Chunk> m_Chunks = new Dictionary<Vector3, Chunk>();

    public Chunk m_ChunkPrototype;

    public int m_ViewDistance = 2;

    public float m_DayTime = 0;
    public ulong m_Seed = 0;
    public string m_WorldName = "Overworld";

    void Start()
    {
       
    }

    void Update()
    {

        
        UpdateChunks_LoadAndUnload(m_ViewDistance, Camera.main.transform.position);

        UpdateChunks_Mesh();
    }

    // 加载和卸载区块，最好做到可以放到多个线程同步执行
    public void UpdateChunks_LoadAndUnload(int viewDistance, Vector3 viewPos)
    {
        Vector3 viewChunkPos = Chunk.ChunkPos(viewPos);

        // Load Chunks
        int n = viewDistance;
        for (int dx = -n; dx <= n; ++dx)
        {
            for (int dy = -n; dy <= n; ++dy) 
            { 
                for (int dz = -n; dz <= n; ++dz)
                {
                    ProvideChunk(new Vector3(dx * 16, dy * 16, dz * 16) + viewChunkPos);
                }
            }
        }

        // Unload Chunks
        int lim = viewDistance * 16;
        List<Chunk> unloadchunks = new List<Chunk>();
        foreach (Chunk chunk in m_Chunks.Values)
        {
            Vector3 p = chunk.Position();
            if (Mathf.Abs(p.x-viewChunkPos.x) > lim ||
                Mathf.Abs(p.y-viewChunkPos.y) > lim || 
                Mathf.Abs(p.z-viewChunkPos.z) > lim) //Vector3.Distance(chunk.Position() + new Vector3(8, 8, 8), viewPos) > viewDistance * 30)
            {
                unloadchunks.Add(chunk);
            }
        }
        foreach (Chunk chunk in unloadchunks)
        {
            UnloadChunk(chunk);
        }
    }

    public void UpdateChunks_Mesh()
    {
        foreach (Chunk chunk in m_Chunks.Values)
        {
            if (chunk.m_Dirty)
            {
                chunk.m_Dirty = false;
                chunk.UpdateMesh();
            }
        }
    }

    public ref Cell GetCell(float3 cellpos)
    {
        Chunk chunk = GetLoadedChunk(Chunk.ChunkPos(cellpos));

        if (chunk == null)
        {
            //Debug.LogWarning("Getting Cell at Non-Loaded Chunk");
            //throw new System.Exception();
            return ref Cell.Nil;
        }

        return ref chunk.LocalCell(Chunk.LocalPos(cellpos));
    }

    public Chunk GetLoadedChunk(float3 chunkpos)
    {
        Assert.IsTrue(chunkpos.x % 16 == 0 && chunkpos.y % 16 == 0 && chunkpos.z % 16 == 0);

        return m_Chunks.GetValueOrDefault(chunkpos);
    }

    public Chunk ProvideChunk(float3 chunkpos)
    {
        Chunk chunk = GetLoadedChunk(chunkpos);

        if (chunk != null)
        {
            return chunk;
        }
        else
        {
            chunk = Instantiate(m_ChunkPrototype, chunkpos, quaternion.identity);  // CreateEntity
            chunk.m_World = this;
            m_Chunks.Add(chunkpos, chunk);
            Debug.Log("New Chunk " + chunkpos);

            // Load Chunk or Generate Chunk

            ChunkGenerator.GenerateChunk(chunk);

            chunk.m_Dirty = true;

            return chunk;
        }
    }

    public void UnloadChunk(Chunk chunk)
    {
        Assert.IsTrue(chunk.m_World == this);
        chunk.m_World = null;
        Debug.Log("Unload Chunk: " + chunk.Position());

        bool removed = m_Chunks.Remove(chunk.Position());
        Destroy(chunk.gameObject);  // DestroyEntity

        Assert.IsTrue(removed);
    }

    public void OnDrawGizmos()
    {
        float3 viewerpos = Camera.main.transform.position;
        float3 viewerChunkPos = Chunk.ChunkPos(viewerpos);

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(viewerChunkPos, Vector3.one * (m_ViewDistance * 2 + 1) * 16.0f);


        //int n = 4;
        //for (int rx = -n; rx <= n; ++rx)
        //{
        //    for (int ry = -n; ry <= n; ++ry)
        //    {
        //        for (int rz = -n; rz <= n; ++rz)
        //        {
        //            Vector3 dp = new(rx, ry, rz);
        //            Vector3 p = Maths.Floor(viewerpos) + dp;

        //            float val = (GetCell(p).Value + 1.0f) * 0.5f;
        //            Gizmos.color = new(val,val,val, 1.0f);
        //            Gizmos.DrawSphere(p, 0.1f);
        //        }
        //    }
        //}
    }
}
