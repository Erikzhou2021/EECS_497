using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
    public float radius;
    public float lineWidth;
    //public float x;
    //public float y;

    private void Start()
    {
        DrawCircleImprint();
    }
    public void DrawCircleImprint()
    {
        var segments = 360;
        var line = gameObject.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3((Mathf.Sin(rad) * radius), 0.1f, (Mathf.Cos(rad) * radius));
        }

        line.SetPositions(points);

        
    }
}