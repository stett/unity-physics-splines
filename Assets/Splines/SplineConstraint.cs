using UnityEngine;
using System.Collections;

public class SplineConstraint : MonoBehaviour {

    public SplineData spline = null;
    public int   vertex_id = 0;
    float vertex_position = 0.0f;
    float vertex_distance = 0.0f;
    ConfigurableJoint joint;

    // Initialize
    void Start() {
        joint = gameObject.AddComponent<ConfigurableJoint> ();
        joint.anchor = new Vector3 (0, 0, 0);
        joint.autoConfigureConnectedAnchor = false;
        joint.xMotion = ConfigurableJointMotion.Free;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
    }
    
    // Update is called once per frame
    void FixedUpdate () {
        ApplyConstraint ();
    }

    // Find the closest spline vertex id and position
    void FindNearestVertex(SplineData spline, int vertex_id, out float vertex_position, out float vertex_distance) {

        // If we don't have a spline, skip this
        vertex_position = 0.0f;
        vertex_distance = 0.0f;
        if (spline == null) return;

        // Get the vertex end-points
        Vector3 v0 = spline.transform.TransformPoint (spline.vertexes[vertex_id]);
        Vector3 v1 = spline.transform.TransformPoint (spline.vertexes[vertex_id+1]);

        // Find the closest point to the vertex segement
        //
        // TODO: Make this actually calculate closest point on the spline!
        //       Is it possible?
        float seg_len_sq = (v1 - v0).sqrMagnitude;
        vertex_position = Vector3.Dot (transform.position - v0, v1 - v0) / seg_len_sq;

        // Get the distance to the vertex
        if (vertex_position < 0.0f) {
            vertex_distance = Vector3.Distance(transform.position, v0);
        } else if (vertex_position > 1.0f) {
            vertex_distance = Vector3.Distance(transform.position, v1);
        } else {
            vertex_distance = Vector3.Distance(transform.position, v0 + (v1 - v0) * vertex_position);
        }
    }

    // Constrain the position to the vertex, and limit the velocity to the direction of the path
    void ApplyConstraint() {

        // Make sure we have a spline
        if (spline == null) return;

        // Update the vertex we should be on
        FindNearestVertex(spline, vertex_id, out vertex_position, out vertex_distance);

        // If we've gone off one of the ends, try to jump to the next vertex
        SplineData next_spline = spline;
        int next_vertex_id = vertex_id;
        if (vertex_position < 0.0f) {
            if (vertex_id > 0) {
                next_vertex_id = vertex_id - 1;
            } else if (spline.in_connections.Count > 0) {
                next_spline = spline.in_connections[0];
                next_vertex_id = spline.in_connections[0].vertexes.Count - 2;
            } else {
                spline = null;
                return;
            }
        } else if (vertex_position > 1.0f) {
            if (vertex_id < spline.vertexes.Count - 2) {
                next_vertex_id = vertex_id + 1;
            } else if (spline.out_connections.Count > 0) {
                next_spline = spline.out_connections[0];
                next_vertex_id = 0;
            } else {
                spline = null;
                return;
            }
        }

        // If we're supposed to try jumping, then fuckin' do it
        if (next_vertex_id != vertex_id || next_spline != spline) {
            float next_vertex_position;
            float next_vertex_distance;
            FindNearestVertex(next_spline, next_vertex_id, out next_vertex_position, out next_vertex_distance);
            if (next_vertex_distance < vertex_distance) {
                spline = next_spline;
                vertex_id = next_vertex_id;
                vertex_position = next_vertex_position;
                vertex_distance = next_vertex_distance;
            }
        }

        // Update the joint axis
        joint.connectedAnchor = spline.transform.TransformPoint(spline.Interpolate(vertex_id, vertex_position));
        joint.axis = spline.transform.TransformDirection(spline.InterpolateNormal(vertex_id, vertex_position));
    }

    // Draw the segment from here to the path we're going to jump to
    void OnDrawGizmos() {
        if (spline == null) return;

        FindNearestVertex(spline, vertex_id, out vertex_position, out vertex_distance);
        Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        
        Vector3 pos = spline.transform.TransformPoint(spline.Interpolate(vertex_id, vertex_position));
        Vector3 norm = spline.transform.TransformDirection(spline.InterpolateNormal(vertex_id, vertex_position));

        Gizmos.DrawLine(transform.position, pos);
        Gizmos.DrawLine(pos, pos + norm);
    }
}
