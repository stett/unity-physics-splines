using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SplineData : MonoBehaviour {
    
    public List<Vector3> vertexes;
    public List<Vector3> tangents;
    public List<SplineData> in_connections;
    public List<SplineData> out_connections;

    // Use this for initialization
    void Start () {
        GenerateTangents ();

        // Hook up inputs/outputs
        foreach (SplineData connection in in_connections)
            if (!connection.out_connections.Contains(this))
                connection.out_connections.Add(this);
        foreach (SplineData connection in out_connections)
            if (!connection.in_connections.Contains(this))
                connection.in_connections.Add(this);
    }
    
    // Update is called once per frame
    void Update () {}

    // Generate tangents if needed
    void GenerateTangents() {

        // If there aren't enough defined tangents, fill them in
        if (tangents.Count < vertexes.Count && vertexes.Count > 0) {
            for (int i = tangents.Count; i < vertexes.Count; i ++) {

                if (i == 0) {
                    tangents.Add ((vertexes[i+1] - vertexes[i]) * .5f);
                } else if (i > 0 && i < vertexes.Count - 1) {
                    tangents.Add ((vertexes[i+1] - vertexes[i-1]) * .5f);
                } else {
                    tangents.Add ((vertexes[i] - vertexes[i-1]) * .5f);
                }
            }
        }

        // If there are too many tangents, delete them
        if (tangents.Count > vertexes.Count) {
            for (int i = vertexes.Count; i < tangents.Count; i ++) {
                tangents.RemoveAt(i);
            }
        }
    }
    
    // Draw the spline path as a gizmo
    void OnDrawGizmos() {

        GenerateTangents ();

        for (int i = 1; i < vertexes.Count; i ++) {

            // Draw the raw vertex/segment
            Gizmos.color = new Color (.8f, .8f, .8f, .2f);
            Vector3 v0 = transform.TransformPoint(vertexes[i-1]);
            Vector3 v1 = transform.TransformPoint(vertexes[i]);
            Gizmos.DrawLine (v0, v1);
            Gizmos.color = new Color (0.5f, 1.0f, 0.7f);
            if (i == 1)
                Gizmos.DrawWireSphere(v0, 0.01f);
            Gizmos.DrawWireSphere(v1, 0.01f);

            // Draw the tangent
            Gizmos.color = new Color (1.0f, 0.5f, 0.2f);
            if (i == 1)
                Gizmos.DrawLine (v0, v0 + transform.TransformDirection(tangents[i-1]));
            Gizmos.DrawLine (v1, v1 + transform.TransformDirection(tangents[i]));

            // Draw the interpolated segment
            Vector3 p0 = v0;
            Vector3 p1;
            int segments = 10;
            
            for (int ii = 1-5; ii <= segments + 5; ii ++) {
                float t = (float)ii / (float)segments;
                Gizmos.color = new Color (0.5f, 1.0f, 0.7f);
                if (t < 0.0f || t > 1.0f)
                    Gizmos.color = new Color (0.5f, 1.0f, 0.7f, 0.3f);
                p1 = transform.TransformPoint (Interpolate (i-1, t));
                Gizmos.DrawLine (p0, p1);
                p0 = p1;
            }
        }

        // Draw end connections
        Gizmos.color = new Color (1.0f, 0.3f, 0.1f);
        foreach (SplineData connection in out_connections) {
            Vector3 v0 = transform.TransformPoint(vertexes[vertexes.Count-1]);
            Vector3 v1 = connection.transform.TransformPoint(connection.vertexes[0]);
            Gizmos.DrawLine(v0, v1);
        }
    }
    
    // Get an interpolated point
    public Vector3 Interpolate(int i, float t) {
        if (t < 0.0f) t = 0.0f; else if (t > 1.0f) t = 1.0f;
        float t2 = t*t;
        float t3 = t*t2;
        return (2*t3 - 3*t2 + 1) * vertexes[i]
             + (t3 - 2*t2 + t) * tangents[i]
             + (-2*t3 + 3*t2) * vertexes[i+1]
             + (t3 - t2) * tangents[i+1];
    }

    // Get the tangent normal vector at a point on the spline
    public Vector3 InterpolateNormal(int i, float t) {
        if (t < 0.0f) t = 0.0f; else if (t > 1.0f) t = 1.0f;
        float t2 = t*t;
        return ((6*t2 - 6*t) * vertexes[i]
              + (3*t2 - 4*t + 1) * tangents[i]
              + (-6*t2 + 6*t) * vertexes[i+1]
              + (3*t2 - 2*t) * tangents[i+1]).normalized;
    }
}
