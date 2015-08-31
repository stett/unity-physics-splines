using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SplineData : MonoBehaviour {
	
	public List<Vector3> vertexes;
	public List<Vector3> tangents;

	// Use this for initialization
	void Start () {
		GenerateTangents ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Generate tangents if needed
	void GenerateTangents() {

		// TEMP
		tangents.Clear ();

		// If there aren't enough defined tangents, fill them in
		if (tangents.Count < vertexes.Count && vertexes.Count > 0) {
			for (int i = tangents.Count; i < vertexes.Count; i ++) {

				if (i == 0) {
					tangents.Add ((vertexes[i+1] - vertexes[i]) * .25f);
				} else if (i > 0 && i < vertexes.Count - 1) {
					tangents.Add ((vertexes[i+1] - vertexes[i-1]) * .33f);
				} else {
					tangents.Add ((vertexes[i] - vertexes[i-1]) * .25f);
				}

				/*if (i < vertexes.Count - 1)
					tangents.Add ((vertexes[i+1] - vertexes[i]) * .5f);
				else
					tangents.Add ((vertexes[i] - vertexes[i-1]) * .5f);
				*/
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
				Gizmos.DrawLine (v0, v0 + transform.TransformPoint (tangents[i-1]));
			Gizmos.DrawLine (v1, v1 + transform.TransformPoint (tangents[i]));

			// Draw the interpolated segment
			Vector3 p0 = v0;
			Vector3 p1;
			Gizmos.color = new Color (0.5f, 1.0f, 0.7f);
			for (float t = 0.1f; t <= 1.1f; t += 0.1f) {
				float t2 = t*t;
				float t3 = t*t2;
				p1 = (2*t3 - 3*t2 + 1) * v0
				   + (t3 - 2*t2 + t) * tangents[i-1]
				   + (-2*t3 + 3*t2) * v1
				   + (t3 - t2) * tangents[i];
				Gizmos.DrawLine (p0, p1);
				p0 = p1;
			}
		}
	}
}
