using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

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


        UpdateChunks_LoadAndUnload(m_ViewDistance, new Vector3(0, 0, 0));

        UpdateChunks_Mesh();
    }

    public void UpdateChunks_LoadAndUnload(int viewDistance, Vector3 viewPos)
    {
        // Load Chunks
        int n = viewDistance;
        for (int dx = -n; dx <= n; ++dx)
        {
            // for (int dy = -n; dy <= n; ++dy)
            for (int dz = -n; dz <= n; ++dz)
            {
                ProvideChunk(new Vector3(dx * 16, 0, dz * 16) + viewPos);
            }
        }

        // Unload Chunks
        Vector3 viewChunkPos = Chunk.ChunkPos(viewPos);
        List<Chunk> unloadChunks = new List<Chunk>();
        foreach (Chunk chunk in m_Chunks.Values)
        {
            // Use Abs.
            if (Vector3.Distance(chunk.Position() + new Vector3(8, 8, 8), viewPos) > viewDistance * 16 * 2)
            {
                unloadChunks.Add(chunk);
            }
        }
        foreach (Chunk chunk in unloadChunks)
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

    public ref Cell GetCell(Vector3 cellpos)
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

    public Chunk GetLoadedChunk(Vector3 chunkpos)
    {
        //int f = ((int)chunkpos.x) % 16;
        //int f1 = (int)chunkpos.y % 16;
        //float f2 = chunkpos.z % 16;
        //int f32 = (int)chunkpos.z % 16;
        //bool va = chunkpos.x % 16 == 0 && chunkpos.y % 16 == 0 && chunkpos.z % 16 == 0;
        //if (!va) {
        //    throw new System.Exception();
        //}
        Assert.IsTrue(chunkpos.x % 16 == 0 && chunkpos.y % 16 == 0 && chunkpos.z % 16 == 0);

        return m_Chunks.GetValueOrDefault(chunkpos);
    }

    public Chunk ProvideChunk(Vector3 chunkpos)
    {
        Chunk chunk = GetLoadedChunk(chunkpos);

        if (chunk != null)
        {
            return chunk;
        }
        else
        {
            chunk = Instantiate(m_ChunkPrototype, chunkpos, Quaternion.identity);  // CreateEntity
            chunk.m_World = this;
            m_Chunks.Add(chunkpos, chunk);
            Debug.Log("New Chunk " + chunkpos);

            // Load Chunk or Generate Chunk

            ChunkGenerator.GenerateChunk(chunk);

            chunk.UpdateMesh();

            return chunk;
        }
    }

    public void UnloadChunk(Chunk chunk)
    {
        Assert.IsTrue(chunk.m_World == this);
        chunk.m_World = null;
        Debug.Log("Chunk Unloaded: " + chunk.Position());

        m_Chunks.Remove(chunk.Position());
        Destroy(chunk);  // DestroyEntity
    }
}
