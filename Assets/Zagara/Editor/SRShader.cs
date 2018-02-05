using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SoftRender
{
    public struct a2v
    {
        public Vector3 postion;
        public Vector3 normal;
        public Vector2 uv;
    }
    public struct v2f
    {
        public Vector3 postion;
        public Vector3 normal;
        public Vector2 uv;
        #region dataIndex
        public float this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return postion.x;
                    case 1:
                        return postion.y;
                    case 2:
                        return postion.z;
                    case 3:
                        return normal.x;
                    case 4:
                        return normal.y;
                    case 5:
                        return normal.z;
                    case 6:
                        return uv.x;
                    case 7:
                        return uv.y;
                }
                return 0f;
            }
            set
            {
                switch (i)
                {
                    case 0:
                        postion.x = value;
                        break;
                    case 1:
                        postion.y = value;
                        break;
                    case 2:
                        postion.z = value;
                        break;
                    case 3:
                        normal.x = value;
                        break;
                    case 4:
                        normal.y = value;
                        break;
                    case 5:
                        normal.z = value;
                        break;
                    case 6:
                        uv.x = value;
                        break;
                    case 7:
                        uv.y = value;
                        break;
                }
            }
        }
        #endregion
    }
    
    Vertex VertShader(a2v a)
    {
        v2f v = new v2f();
        v.postion = L2WMat.MultiplyPoint3x4(a.postion);
        v.normal = L2WMat.MultiplyVector(a.normal);
        v.uv = a.uv;
        Vertex vert = new Vertex();
        Vector4 svp = a.postion;
        svp.w = 1f;
        svp = MVPMat * svp;
        vert.data = v;
        vert.x = (svp.x / svp.w / 2 + 0.5f) * width;
        vert.y = (svp.y / svp.w / 2 + 0.5f) * height;
        vert.z = svp.z / svp.w / 2 + 0.5f;
        return vert;
    }

    SRTexture dotLightAtten = new SRTexture("Assets/SoftRender/Resources/dotlight.jpg");

    Color FragShader(v2f v)
    {
        float r = 0f, g = 0f, b = 0f;
        float dis = 0f;
        float atten = 0f;
        Vector3 lightDir;
        foreach (var light in lights)
        {
            switch (light.type)
            {
                case LightType.Directional:
                    lightDir = -light.transform.forward;
                    atten = Vector3.Dot(Vector3.Normalize(lightDir), Vector3.Normalize(v.normal));
                    atten = Mathf.Max(0, atten);
                    r += light.color.r * atten;
                    g += light.color.g * atten;
                    b += light.color.b * atten;
                    break;
                case LightType.Point:
                    dis = Vector3.Distance(light.transform.position, v.postion);
                    if (dis > light.range) continue;
                    atten = dotLightAtten[(int)(dis / light.range), 2].r;
                    lightDir = light.transform.position - v.postion;
                    atten *= Vector3.Dot(Vector3.Normalize(lightDir), Vector3.Normalize(v.normal));
                    atten *= light.intensity;
                    r += light.color.r * atten;
                    g += light.color.g * atten;
                    b += light.color.b * atten;
                    break;
            }            
        }
        return new Color(r, g, b);
    }

    SRTexture MainTexture = new SRTexture("Assets/SoftRender/Resources/Explorer_High.PNG");
    Color FragShader_Human(v2f v)
    {
        float r = 0f, g = 0f, b = 0f;
        float atten = 0f;
        Vector3 lightDir;
        Light light = lights[0];
        Color texture = MainTexture[v.uv.x, v.uv.y];
        lightDir = -light.transform.forward;
        atten = Vector3.Dot(Vector3.Normalize(lightDir), Vector3.Normalize(v.normal));
        atten = Mathf.Max(0, atten);
        r = light.color.r * atten * texture.r;
        g = light.color.g * atten * texture.g;
        b = light.color.b * atten * texture.b;
        return new Color(r, g, b);
    }

    Color FragShader_Human_T(v2f v)
    {
        Vector3 viewDir = camera.transform.position - v.postion;
        float rim = 1 - Mathf.Max(0, Vector3.Dot(Vector3.Normalize(v.normal), Vector3.Normalize(viewDir)));
        float rimPower = Mathf.Pow(rim, 1 / 0.55f);
        Color texture = MainTexture[v.uv.x, v.uv.y];
        float rimr = 1f * rimPower + texture.r * 0.2f;
        float rimg = 0.77f * rimPower + texture.g * 0.2f;
        float rimb = 0.77f * rimPower + texture.b * 0.2f;
        return new Color(rimr, rimg, rimb, 0.77f);
    }
}