using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using Unity.Jobs;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using UnityEngine.Pool;
using System.Linq;

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

















//[ExecuteInEditMode]
public class World : MonoBehaviour
{
    public World()
    {
        Log.info("World Object Created. ");
    }
    static World()
    {
        Log.info("World StaticNew();");
    }

    public Dictionary<float3, Chunk> m_Chunks = new Dictionary<float3, Chunk>();

    public Chunk m_ChunkPrototype; 

    [Range(0, 16)]
    public int m_ViewDistance = 2;

    [Header("WorldInfo")]

    [Range(0, 1)]
    public float m_DayTime = 0;
    public ulong m_Seed = 0;
    public string m_WorldName = "Overworld";

    public static World _Inst;

    [Header("Debug Draw")]
    public bool m_DbgDrawViewDistanceBound = true;
    public bool m_DbgDrawChunkBound = true;
    public string m_DbgWorldInfoStr = "";

    public bool m_LoadChunks = true;


    void Start()
    {
        Log.assert(_Inst == null);
        _Inst = this;

        Ethertia.m_SomeData.Add(1, 2);
        Log.info("World Starts. "+ this.GetHashCode());

        int workThreads, cpThreads;
        ThreadPool.GetMaxThreads(out workThreads, out cpThreads);
        Log.info("ThreadPool Max "+ workThreads+", "+ cpThreads);

        //for (int i = 0; i < 1000; ++i)
        //{
        //    Task t = new Task(() => 
        //    {
        //        int TiD = i;
        //        int j = 0;
        //        while (true)
        //        {
        //            if (m_LoadChunks) Thread.Sleep(9999);
        //            math.inverse(new float4x4(999));

        //            Log.info($"-----------THREAD {TiD} x {j}");
        //            ++j;
        //        }
        //    });
        //    t.Start();
        //}
    }

    void Awake()
    {

        Log.info("World Awake.");
    }



    void Update()
    {
        float DayLengthSec = 60 * 1;
        m_DayTime += Time.deltaTime / DayLengthSec;
        
        if (m_LoadChunks)
        {
            UpdateChunks_LoadAndUnload(m_ViewDistance, Camera.main.transform.position);
        }

        UpdateChunks_Mesh();
    }

    void OnDestroy()
    {
    }

    void OnEnable()
    {
        Log.info($"World Enable @{GetHashCode()}, Chunks: {m_Chunks.Count}");
    }

    void OnDisable()
    {
        Log.info($"Unload All Chunks. ({m_Chunks.Count})");
        UnloadAllChunks();

        Log.info($"World Disable @{GetHashCode()}, Chunks: {m_Chunks.Count}");
    }



    public void OnDrawGizmos()
    {
        float3 viewerpos = Camera.main.transform.position;
        float3 viewerChunkPos = Chunk.ChunkPos(viewerpos);

        if (m_DbgDrawViewDistanceBound)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(viewerChunkPos + 8.0f, Vector3.one * (m_ViewDistance * 2 + 1) * 16.0f);
        }
        if (m_DbgDrawChunkBound)
        {
            Gizmos.color = Color.gray;
            foreach (float3 p in m_Chunks.Keys)
            {
                Gizmos.DrawWireCube(p + 8.0f, Vector3.one * 16.0f);
            }
        }

        m_DbgWorldInfoStr = 
            $"LoadedChunks: {m_Chunks.Count}\n" +
            $"ChunksLoading: {m_LoadingChunks.Count}\n" +
            $"ChunksMeshGen: {m_MeshGenTasks.Count}\n" +
            $"VertexData Pool N: {g_Pool_VertexData.CountInactive}";


        //Handles.Label(Vector3.one, m_DbgWorldInfoStr);

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

    //void OnGUI()
    //{

    //    GUI.Label(new Rect(100, 100, 100, 100), "Chunks: " + m_DbgWorldInfoStr);
    //}

    public int m_ChunkGenMaxConcurrent = 10;
    private Dictionary<float3, Task<Chunk>> m_LoadingChunks = new Dictionary<float3, Task<Chunk>>();

    // 加载和卸载区块，最好做到可以放到多个线程同步执行
    public void UpdateChunks_LoadAndUnload(int viewDistance, float3 viewPos)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        float3 viewChunkPos = Chunk.ChunkPos(viewPos);

        // Load Chunks
        int n = viewDistance;
        for (int dx = -n; dx <= n; ++dx)
        {
            for (int dy = -n; dy <= n; ++dy) 
            {
                for (int dz = -n; dz <= n; ++dz)
                {
                    float3 chunkpos = new float3(dx * 16, dy * 16, dz * 16) + viewChunkPos;

                    if (GetLoadedChunk(chunkpos) != null || m_LoadingChunks.ContainsKey(chunkpos))
                        continue;

                    Chunk chunk = Instantiate(m_ChunkPrototype, chunkpos, Quaternion.identity, gameObject.transform);  // CreateEntity
                    chunk.m_World = this;
                    chunk.chunkpos = chunkpos;
                    chunk.m_Dirty = true;

                    Task<Chunk> task = new Task<Chunk>(() =>
                    {
                        // Debug.Log("New Chunk " + chunkpos);

                        // Load Chunk or Generate Chunk

                        ChunkGenerator.GenerateChunk(chunk);

                        return chunk;
                    });
                    task.Start();

                    m_LoadingChunks.Add(chunkpos, task);

                    if (m_LoadingChunks.Count > m_ChunkGenMaxConcurrent)
                        goto afterChunkLoad;
                }
            }
        }
        afterChunkLoad:


        List<float3> completedLoadChunks = new List<float3>();
        foreach (var kv in m_LoadingChunks)
        {
            Task<Chunk> task = kv.Value;
            if (task.IsCompleted)
            {
                m_Chunks.Add(kv.Key, task.Result);
                completedLoadChunks.Add(kv.Key);
            }
        }
        if (completedLoadChunks.Count > 0)
            Log.warn($"{completedLoadChunks.Count}/{m_LoadingChunks.Count} Chunks Load Completed.");
        foreach (var k in completedLoadChunks)
        {
            m_LoadingChunks.Remove(k);
        }

        // Unload Chunks
        int lim = viewDistance * 16;
        List<Chunk> unloadchunks = new List<Chunk>();
        foreach (Chunk chunk in m_Chunks.Values)
        {
            float3 p = chunk.chunkpos;
            if (Mathf.Abs(p.x-viewChunkPos.x) > lim ||
                Mathf.Abs(p.y-viewChunkPos.y) > lim || 
                Mathf.Abs(p.z-viewChunkPos.z) > lim) //Vector3.Distance(chunk.Position() + new Vector3(8, 8, 8), viewPos) > viewDistance * 30)
            {
                unloadchunks.Add(chunk);

                // tmp: prevents main-thread lag
                if (unloadchunks.Count > 30)
                    break;
            }
        }
        foreach (Chunk chunk in unloadchunks)
        {
            UnloadChunk(chunk);
        }

        if (stopwatch.Elapsed.TotalMilliseconds > 3)
            Log.warn("Chunk Gen Unload Used "+ stopwatch.Elapsed.TotalMilliseconds +"ms");
    }

    public struct TaskHandle
    {
        public float3 chunkpos;  // 不直接挂 Chunk指针，因为当Mesh处理完后，可能这个区块已被卸载、销毁掉了，再访问就会出错。因此存chunkpos 验证获取后再更新
        public VertexData.MeshData result;
        public Task task;
    }


    public int m_MeshGenMaxConcurrent = 10;
    public List<TaskHandle> m_MeshGenTasks = new List<TaskHandle>();

    public IObjectPool<VertexData> g_Pool_VertexData = new ObjectPool<VertexData>(() => new VertexData(2048));

    public void UpdateChunks_Mesh()
    {
        BenchmarkTimer rt__ = new BenchmarkTimer();

        int numJobsScheduled = 0;
        foreach (Chunk chunk in m_Chunks.Values)
        {
            if (chunk.m_Dirty)
            {
                chunk.m_Dirty = false;  // m_MeshStat = MESHING;



                //if (g_Pool_VertexData.Count == 0)
                //    g_Pool_VertexData.AddLast(new VertexData(2048));
                VertexData vtx = g_Pool_VertexData.Get();
                //g_Pool_VertexData.RemoveLast();
                vtx.Clear();

                VertexData.MeshData meshData = new VertexData.MeshData();


                Task task = new Task(() =>
                {
                    //BenchmarkTimer tm = new BenchmarkTimer();
                    //UnityEngine.Profiling.Profiler.BeginSample("[Et] MeshGen");


                    // Generate Mesh
                    ChunkMesher.GenerateMesh(chunk, vtx);

                    vtx.Export(meshData);


                //if (new System.Random().NextDouble() > 0.6)
                //    Thread.Sleep((int)(new System.Random().NextDouble() * 10 * 1000));
                //Thread.Sleep(8 * 1000);
                //UnityEngine.Profiling.Profiler.EndSample();

                //tm.Stop();
                //Log.info("Chunk " + chunk.chunkpos + " Mesh Generated, VertexCount: " + vtx.VertexCount());// + " used " + tm.Elapsed.TotalMilliseconds);
                });
                task.Start();

                m_MeshGenTasks.Add(new() { chunkpos =chunk.chunkpos, task=task, result= meshData });
                ++numJobsScheduled;

                if (m_MeshGenTasks.Count > m_MeshGenMaxConcurrent)
                    break;
            }
        }
        if (numJobsScheduled != 0)
            Log.warn($"{numJobsScheduled} MeshGen Tasks Scheduled.");


        double timerProcessMeshAssign = 0;

        List<TaskHandle> completedTasks = new List<TaskHandle>();
        foreach (TaskHandle handle in m_MeshGenTasks)
        {
            if (handle.task.IsCompleted)
            {
                BenchmarkTimer tm = new BenchmarkTimer();

                Mesh mesh = handle.result.ToMesh();

                //mesh.RecalculateNormals();

                Chunk chunk = GetLoadedChunk(handle.chunkpos);
                if (chunk != null)
                {
                    chunk.UpdateMesh(mesh);
                }

                tm.Stop();
                timerProcessMeshAssign += tm.Elapsed.TotalMilliseconds;
                //Log.info("Chunk Assign Mesh, VertexCount: " + mesh.vertexCount + " used " + tm.Elapsed.TotalMilliseconds);

                // Restore to Pool
                g_Pool_VertexData.Release(handle.result.vtx);

                completedTasks.Add(handle);

                if (timerProcessMeshAssign > 2.0)
                {
                    Log.warn("!!!Mesh Complete Assign Abort, OutOfTimeLimit of 2ms, ="+ timerProcessMeshAssign);
                    break;
                }
            }
        }
        foreach (var taskHandle in completedTasks)
        {
            m_MeshGenTasks.Remove(taskHandle);
        }

        rt__.Stop();
        if (completedTasks.Count != 0)
            Log.warn($"{completedTasks.Count} MeshGen Tasks Completed. TotalMeshUpdate Used: {rt__.Elapsed.TotalMilliseconds}ms");
    }

    // Risk: Untrackable Modify
    public ref Cell GetCell(float3 cellpos)
    {
        Chunk chunk = GetLoadedChunk(Chunk.ChunkPos(cellpos));

        if (chunk == null)
        {
            Cell.Nil.FeaturePoint = float.NaN;
            Cell.Nil.Value = float.NaN;
            return ref Cell.Nil;
        }

        return ref chunk.LocalCell(Chunk.LocalPos(cellpos));
    }

    public bool GetCell(float3 cellpos, out Cell cell)
    {
        Chunk chunk = GetLoadedChunk(Chunk.ChunkPos(cellpos));

        if (chunk == null)
        {
            cell = Cell.Nil;
            return false;
        }

        cell = chunk.LocalCell(Chunk.LocalPos(cellpos));
        return true;
    }

    public Chunk GetLoadedChunk(float3 chunkpos)
    {
        Log.assert(chunkpos.x % 16 == 0 && chunkpos.y % 16 == 0 && chunkpos.z % 16 == 0, "Illegal ChunkPos.");

        return m_Chunks.GetValueOrDefault(chunkpos);
    }


    // Chunk ProvideChunk(float3 chunkpos);  // Removed: 没有使用案例、理由。区块加载应该由专门的加载系统负责，而不是被随机的调用引诱出来。


    public void UnloadChunk(Chunk chunk)
    {
        Log.assert(chunk.m_World == this, "UnloadChunk Error: Unloading Non-Loaded Chunk.");
        chunk.m_World = null;
        Log.info("Unload Chunk: " + chunk.chunkpos);

        bool removed = m_Chunks.Remove(chunk.chunkpos);
        DestroyImmediate(chunk.gameObject);  // DestroyEntity

        Log.assert(removed, "UnloadChunk Error: Unable to remove the chunk.");
    }

    public void UnloadAllChunks()
    {
        foreach (Chunk chunk in new List<Chunk>(m_Chunks.Values))
        {
            UnloadChunk(chunk);
        }
    }


}
