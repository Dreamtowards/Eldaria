

using Unity.Jobs;
using UnityEngine;

namespace Ethertia
{
    public struct ChunkMeshGenJob : IJob
    {
        public Chunk in_Chunk;

        public Mesh out_Mesh;

        public void Execute()
        {
            //VertexData vtx = new VertexData();

            //UnityEngine.Profiling.Profiler.BeginSample("[Et] MeshGen");

            //// Generate Mesh
            //ChunkMesher.GenerateMesh(in_Chunk, vtx);


            //vtx.Export(out_Mesh);
            //out_Mesh.triangles = Maths.Sequence(vtx.VertexCount());

            ////mesh.RecalculateNormals();

            //UnityEngine.Profiling.Profiler.EndSample();
        }
    }
}