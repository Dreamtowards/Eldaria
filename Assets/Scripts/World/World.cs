using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using Unity.Jobs;

public enum VoxelOp
{
    Add,  // Place
    Sub,  // Break
    Set,  // Paint/Replace

    Smooth,
    Sharpen
}

public enum VoxelOpBrush
{
    Sphere,
    Cube
}


namespace Ethertia
{
    //[CustomEditor(typeof(World))]
    //[CanEditMultipleObjects]
    public class WorldEditor : UnityEditor.Editor
    {
        [MenuItem("Tools/Ethertia/Test")]
        public static void DoSth()
        {

        }

        void OnEnable()
        {

        }

        private void OnSceneGUI()
        {

        }

        int m_EditorActiveTab = 0;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            m_EditorActiveTab = GUILayout.Toolbar(m_EditorActiveTab, new[] {
                EditorGUIUtility.TrTextContentWithIcon("Edit", "d_TerrainInspector.TerrainToolSplat"),
                EditorGUIUtility.TrTextContentWithIcon("Settings", "d_TerrainInspector.TerrainToolSettings"),
                EditorGUIUtility.TrTextContentWithIcon("Help", "_Help")
            });
            switch (m_EditorActiveTab)
            {
                case 0:
                    break;
            }

            EditorGUILayout.HelpBox("This script allows Digger to work at runtime, for real-time / in-game digging.\n\n" +
                                    "It has a public method named 'Modify' that you must call from your scripts to edit the terrain.\n\n" +
                                    "See 'DiggerRuntimeUsageExample.cs' in Assets/Digger/Demo for a working example.", MessageType.Info);

            EditorGUILayout.LabelField("Sth");

            //EditorGUILayout.BeginVertical();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editing", EditorStyles.boldLabel);

            OnInspectorGUIHelpTab();

            serializedObject.ApplyModifiedProperties();
        }

        public void OnInspectorGUIHelpTab()
        {
            EditorGUILayout.HelpBox("Thanks for using Digger!\n\n" +
                                    "Need help? Checkout the documentation and join us on Discord to get support!\n\n" +
                                    "Want to help the developer and support the project? Please write a review on the Asset Store!",
                MessageType.Info);


            if (GUILayout.Button("Open documentation"))
            {
                Application.OpenURL("https://ofux.github.io/Digger-Documentation/");
            }

            if (GUILayout.Button("Open Digger Asset Store page"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/terrain/digger-terrain-caves-overhangs-135178");
            }

            if (GUILayout.Button("Open Digger PRO Asset Store page"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/terrain/digger-pro-149753");
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Support is on Discord:", EditorStyles.boldLabel, GUILayout.Width(140));
            var style = new GUIStyle(EditorStyles.textField);
            EditorGUILayout.SelectableLabel("https://discord.gg/C2X6C6s", style, GUILayout.Height(18));
            EditorGUILayout.EndHorizontal();
        }
    }
}


public class World : MonoBehaviour
{
    private Dictionary<int3, Chunk> m_Chunks = new Dictionary<int3, Chunk>();

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
    public void UpdateChunks_LoadAndUnload(int viewDistance, float3 viewPos)
    {
        float3 viewChunkPos = Chunk.ChunkPos(viewPos);

        // Load Chunks
        int n = viewDistance;
        for (int dx = -n; dx <= n; ++dx)
        {
            for (int dy = -n; dy <= n; ++dy) 
            { 
                for (int dz = -n; dz <= n; ++dz)
                {
                    ProvideChunk(new int3(new float3(dx * 16, dy * 16, dz * 16) + viewChunkPos));
                }
            }
        }

        // Unload Chunks
        int lim = viewDistance * 16;
        List<Chunk> unloadchunks = new List<Chunk>();
        foreach (Chunk chunk in m_Chunks.Values)
        {
            float3 p = chunk.Position();
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

    public Chunk GetLoadedChunk(int3 chunkpos)
    {
        Assert.IsTrue(chunkpos.x % 16 == 0 && chunkpos.y % 16 == 0 && chunkpos.z % 16 == 0);

        return m_Chunks.GetValueOrDefault(chunkpos);
    }

    public Chunk ProvideChunk(int3 chunkpos)
    {
        Chunk chunk = GetLoadedChunk(chunkpos);

        if (chunk != null)
        {
            return chunk;
        }
        else
        {
            chunk = Instantiate(m_ChunkPrototype, new float3(chunkpos), Quaternion.identity);  // CreateEntity
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

        bool removed = m_Chunks.Remove(new int3(chunk.Position()));
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
