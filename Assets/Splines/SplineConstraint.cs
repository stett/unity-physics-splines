using UnityEngine;
using System.Collections;

public class SplineConstraint : MonoBehaviour {

	public SplineData spline;
	Rigidbody body;
	int   	vertex_id = 0;
	float 	vertex_position;
	Vector3 vertex_normal;

	// Initialize
	void Start() {
		body = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void Update () {
		ApplyConstraint ();
	}

	// Find the closest spline vertex id and position
	void FindNearestVertex() {
		float min_dist = Mathf.Infinity;
		for (int i = 1; i < spline.vertexes.Count; i ++) {
			float dist = min_dist;
			Vector3 v0 = spline.transform.TransformPoint (spline.vertexes[i-1]);
			Vector3 v1 = spline.transform.TransformPoint (spline.vertexes[i]);
			float seg_len_sq = (v1 - v0).sqrMagnitude;
			
			// If the segment has no length, return the position and distance
			// to the first vertex.
			if (seg_len_sq == 0.0f) {
				dist = Vector3.Distance (transform.position, v0);
				if (dist < min_dist) {
					min_dist = dist;
					vertex_id = i-1;
					vertex_position = 0.0f;
					//vertex_normal = (v1 - v0).normalized;
				}
			} else {
				float t = Vector3.Dot (transform.position - v0, v1 - v0) / seg_len_sq;
				
				// If we are past the first vertex, return the position and
				// distance to the first vertex.
				if (t < 0.0f) {
					dist = Vector3.Distance(transform.position, v0);
					if (dist < min_dist) {
						min_dist = dist;
						vertex_id = i-1;
						vertex_position = 0.0f;
						//vertex_normal = (v1 - v0).normalized;
					}
					
				// If we are past the last vertex, same deal.
				} else if (t > 1.0f) {
					dist = Vector3.Distance (transform.position, v1);
					if (dist < min_dist) {
						min_dist = dist;
						vertex_id = i-1;
						vertex_position = 1.0f;
						//vertex_normal = (v1 - v0).normalized;
					}
					
				// We're right in the sweet spot.
				// Project the position onto the segment.
				} else {
					Vector3 projection = v0 + t * (v1 - v0);
					dist = Vector3.Distance(transform.position, projection);
					if (dist < min_dist) {
						min_dist = dist;
						vertex_id = i-1;
						vertex_position = t;
						//vertex_normal = (v1 - v0).normalized;
					}
				}
			}
		}

		// Compute the normal along the spline

	}

	// Constrain the position to the vertex, and limit the velocity to the direction of the path
	void ApplyConstraint() {

		// Update the vertex we should be on
		FindNearestVertex ();

		// Limit the position
		transform.position = spline.transform.TransformPoint (spline.Interpolate (vertex_id, vertex_position));

		// Limit the velocity
		vertex_normal = spline.transform.TransformPoint (spline.InterpolateNormal (vertex_id, vertex_position)).normalized;
		body.velocity = vertex_normal * Vector3.Dot (body.velocity, vertex_normal);
	}

	// Draw the segment from here to the path we're going to jump to
	void OnDrawGizmos() {
		FindNearestVertex ();
		Gizmos.color = new Color (1.0f, 1.0f, 1.0f, 0.5f);

		Vector3 pos = spline.transform.TransformPoint (spline.Interpolate (vertex_id, vertex_position));
		Vector3 norm = spline.transform.TransformPoint (spline.InterpolateNormal (vertex_id, vertex_position)).normalized;

		Gizmos.DrawLine (transform.position, pos);
		Gizmos.DrawLine (pos, norm);
	}
}
