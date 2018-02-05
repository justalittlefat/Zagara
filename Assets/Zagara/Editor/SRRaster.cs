using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SoftRender
{   
    public List<Fragment> Rast(Triangle t)
    {        
        int xMin = (int)Mathf.Min(t[0].x, t[1].x, t[2].x);
        int xMax = (int)Mathf.Max(t[0].x, t[1].x, t[2].x);
        int yMin = (int)Mathf.Min(t[0].y, t[1].y, t[2].y);
        int yMax = (int)Mathf.Max(t[0].y, t[1].y, t[2].y);
        var fragList = new List<Fragment>((xMax-xMin)*(yMax-yMin));
        for (int m = xMin; m < xMax + 1; m++)
        {
            for (int n = yMin; n < yMax + 1; n++)
            {
                if (m < 0 || m > width - 1 || n < 0 || n > height - 1) continue;
                if (!isLeftPoint(t[0], t[1], m + 0.5f, n + 0.5f)) continue;
                if (!isLeftPoint(t[1], t[2], m + 0.5f, n + 0.5f)) continue;
                if (!isLeftPoint(t[2], t[0], m + 0.5f, n + 0.5f)) continue;
                var frag = new Fragment();
                frag.x = m;
                frag.y = n;
                LerpFragment(t[0], t[1], t[2], frag);
                fragList.Add(frag);
            }
        }
        return fragList;
    }

    public bool isLeftPoint(Vertex a, Vertex b, float x,float y)
    {
        float s = (a.x - x) * (b.y - y) - (a.y - y) * (b.x - x);
        return s > 0 ? false : true;
    }
    
    public void LerpFragment(Vertex a, Vertex b, Vertex c, Fragment frag)
    {
        for(int i = 0; i < 8; i++)
        {
            frag.data[i] = LerpValue(a.data[i], a.x, a.y, b.data[i], b.x, b.y, c.data[i], c.x, c.y, frag.x, frag.y);
        }
        frag.z = LerpValue(a.z, a.x, a.y, b.z, b.x, b.y, c.z, c.x, c.y, frag.x, frag.y);
    }

    float LerpValue(float f1, float x1,float y1, float f2, float x2,float y2, float f3, float x3,float y3
        ,float fragx,float fragy)
    {
        float left = (f1 * x2 - f2 * x1) / (y1 * x2 - y2 * x1) - (f1 * x3 - f3 * x1) / (y1 * x3 - y3 * x1);
        float right = (x2 - x1) / (y1 * x2 - y2 * x1) - (x3 - x1) / (y1 * x3 - y3 * x1);
        float c = left / right;
        left = (f1 * x2 - f2 * x1) / (x2 - x1) - (f1 * x3 - f3 * x1) / (x3 - x1);
        right = (y1 * x2 - y2 * x1) / (x2 - x1) - (y1 * x3 - y3 * x1) / (x3 - x1);
        float b = left / right;
        float a = (f1 - f3 - b * (y1 - y3)) / (x1 - x3);
        return fragx * a + fragy * b + c;
    }
}