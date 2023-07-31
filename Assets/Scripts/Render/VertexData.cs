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

    public List<Vertex> Vertices = new List<Vertex>();
    public List<int> Indices = new List<int>();

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

    public void Export(Mesh mesh)
    {
        int vc = VertexCount();
        Vector3[] pos  = new Vector3[vc];
        Vector2[] tex  = new Vector2[vc];
        Vector3[] norm = new Vector3[vc];

        for (int i = 0; i < vc; ++i)
        {
            // how by ref?
            Vertex vtx = IsIndexed() ? Vertices[Indices[i]] : Vertices[i];
            pos[i] = vtx.Position;
            tex[i] = vtx.TexCoord;
            norm[i] = vtx.Normal;
        }

        mesh.vertices = pos;
        mesh.uv = tex;
        mesh.normals = norm;
    }

}