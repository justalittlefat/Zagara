using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SRInspector))]
[ExecuteInEditMode]
public partial class SoftRender : Editor {

    SRInspector sri;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Fire!"))
        {
            Run();
        }
    }

    public void Run()
    {
        SRInspector t = (SRInspector)target;

        sri = target as SRInspector;
        var ml = sri.GetComponentsInChildren<MeshFilter>();
        var ll = sri.GetComponentsInChildren<Light>();
        var c = sri.GetComponentInChildren<Camera>();

        var sr = new SoftRender(c, ll, ml,t.SavePath,t.PicName);
        sr.DrawFrame();
        //sr.DrawFrame_Human();
    }
}
