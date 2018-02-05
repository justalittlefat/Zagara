using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SoftRender
{
    const int width = 1280;
    const int height = 720;
    SRTexture frameBuffer;
    float[,] depthBuffer = new float[width, height];
    bool blend;

    Camera camera;
    Light[] lights;
    MeshFilter[] meshs;

    VAO vao;
    List<Vertex> vertexList;
    List<Triangle> triangleList;
    Matrix4x4 L2WMat;
    Matrix4x4 MVPMat;
    Func<v2f,Color> usedShader;

    public SoftRender(Camera camera, Light[] lights, MeshFilter[] meshs,string savePath,string picName)
    {
        this.camera = camera;
        this.lights = lights;
        this.meshs = meshs;    
        vertexList = new List<Vertex>(1024 * 4);
        triangleList = new List<Triangle>(1024 * 4);
        frameBuffer = new SRTexture(width, height,savePath,picName);
    }

    public void DrawFrame()
    {
        frameBuffer.Clear(0.2f,0.3f,0.4f);
        usedShader = FragShader;
        foreach (var mesh in meshs)
        {
            L2WMat = mesh.transform.localToWorldMatrix;
            MVPMat = camera.projectionMatrix * camera.worldToCameraMatrix * L2WMat;
            vao = new VAO(mesh);
            DrawElement();
            vertexList.Clear();
            triangleList.Clear();
        }
        frameBuffer.Save();
    }

    public void DrawFrame_Human()
    {
        frameBuffer.Clear(0.2f, 0.3f, 0.4f);

        usedShader = FragShader_Human;
        L2WMat = meshs[0].transform.localToWorldMatrix;
        MVPMat = camera.projectionMatrix * camera.worldToCameraMatrix * L2WMat;
        vao = new VAO(meshs[0]);
        DrawElement();
        vertexList.Clear();
        triangleList.Clear();

        blend = true;
        usedShader = FragShader_Human_T;
        L2WMat = meshs[1].transform.localToWorldMatrix;
        MVPMat = camera.projectionMatrix * camera.worldToCameraMatrix * L2WMat;
        vao = new VAO(meshs[1]);
        DrawElement();
        vertexList.Clear();
        triangleList.Clear();

        frameBuffer.Save();
    }

    public void DrawElement()
    {
        RunVertexShader();
        TriangleSetup(); 
        Rasterization();
    }
    
    public void RunVertexShader()
    {
        for (int i = 0; i < vao.vbo.Length; i++)
        {
            a2v a = vao.vbo[i];
            Vertex v = VertShader(a);
            vertexList.Add(v);
        }
    }

    public void TriangleSetup()
    {
        for(int i = 0; i < vao.ebo.Length; i+=3)
        {
            Triangle t = new Triangle(
                vertexList[vao.ebo[i]], 
                vertexList[vao.ebo[i+1]], 
                vertexList[vao.ebo[i+2]]
                );
            triangleList.Add(t);
        }
    }

    public void Rasterization()
    {
        for (int i = 0; i < triangleList.Count; i++)
        {
            var fl = Rast(triangleList[i]);
            foreach (var frag in fl)
            {
                if (frag.z > depthBuffer[frag.x, frag.y] && depthBuffer[frag.x, frag.y] != 0) continue;
                Color col = usedShader(frag.data);
                if (blend)
                {
                    Color t = frameBuffer[frag.x, frag.y];
                    float r = t.r * (1 - col.a) + col.r * col.a;
                    float g = t.g * (1 - col.a) + col.g * col.a;
                    float b = t.b * (1 - col.a) + col.b * col.a;
                    frameBuffer[frag.x, frag.y] = new Color(r, g, b); 
                }
                else
                {
                    frameBuffer[frag.x, frag.y] = col;
                }
                depthBuffer[frag.x, frag.y] = frag.z;
            }
        }
    }
}