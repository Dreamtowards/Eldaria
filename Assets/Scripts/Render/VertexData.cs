using System.Collections.Generic;
using UnityEngine;

public class VertexData
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector2 TexCoord;
        public Vector3 Normal;

        public Vertex(Vector3 pos, Vector2 tex, Vector3 norm)
        {
            Position = pos;
            TexCoord = tex;
            Normal = norm;
        }
    }

    public List<Vertex> Vertices;
    public List<int> Indices;

    public VertexData(int initCap = 0)
    {
        Vertices = new List<Vertex>(initCap);
        Indices = new List<int>(initCap);
    }

    public static void MakeIndexed(VertexData vtx)
    {
        // impl.
    }

    public bool IsIndexed()
    {
        return Indices.Count > 0;
    }

    public int VertexCount()
    {
        return IsIndexed() ? Indices.Count : Vertices.Count;
    }

    public void AddVertex(Vector3 pos, Vector2 tex, Vector3 norm)
    {
        Vertices.Add(new Vertex(pos, tex, norm));
    }

    public void RemoveVertex(int index, int count = 1)
    {
        Vertices.RemoveRange(index, count);
    }

    public void Clear()
    {
        Vertices.Clear();
        Indices.Clear();
    }

    public class MeshData
    {
        public Vector3[] pos;
        public Vector2[] uv;
        public Vector3[] norm;
        public int[] indices;

        public VertexData vtx;

        public Mesh ToMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = pos;
            mesh.uv = uv;
            mesh.normals = norm;
            mesh.triangles = indices;
            return mesh;
        }
    }

    public void Export(MeshData md)
    {
        int vc = VertexCount();
        md.pos  = new Vector3[vc];
        md.uv  = new Vector2[vc];
        md.norm = new Vector3[vc];
        md.vtx = this;  // tmp

        for (int i = 0; i < vc; ++i)
        {
            // how by ref?
            Vertex vtx = IsIndexed() ? Vertices[Indices[i]] : Vertices[i];
            md.pos[i] = vtx.Position;
            md.uv[i] = vtx.TexCoord;
            md.norm[i] = vtx.Normal;
        }

        md.indices = new int[vc];
        for (int i = 0; i < vc; ++i)
        {
            md.indices[i] = i;
        }
    }

    //public void Export(Mesh mesh)
    //{
    //    Vector3[] pos;
    //    Vector2[] uv;
    //    Vector3[] norm;
    //    Export(out pos, out uv, out norm);

    //    mesh.vertices = pos;
    //    mesh.uv = uv;
    //    mesh.normals = norm;
    //}

}