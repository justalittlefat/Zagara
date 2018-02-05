using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SoftRender
{
    public class VAO
    {
        public a2v[] vbo;
        public int[] ebo;
        public VAO(MeshFilter mf)
        {
            Mesh m = mf.sharedMesh;
            vbo = new a2v[m.vertexCount];
            for (int i = 0; i < m.vertexCount; i++)
            {
                a2v a = new a2v();
                a.postion = m.vertices[i];
                a.normal = m.normals[i];
                a.uv = m.uv[i];
                vbo[i] = a;
            }
            ebo = m.triangles;
        }
    }

    public class Triangle
    {
        readonly Vertex[] verts;
        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            verts = new Vertex[3];
            verts[0] = a;
            verts[1] = b;
            verts[2] = c;
        }
        public Vertex this[int index]
        {
            get
            {
                return verts[index];
            }
        }
    }

    public class Vertex
    {
        public float x;
        public float y;
        public float z;
        public v2f data;        
    }

    public class Fragment
    {
        public int x;
        public int y;
        public float z;
        public v2f data;
        public float fx
        {
            get
            {
                return x + 0.5f;
            }
        }
        public float fy
        {
            get
            {
                return y + 0.5f;
            }
        }
    }
}