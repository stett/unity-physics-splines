using UnityEngine;
using System.Collections;


public class SplineData : MonoBehaviour {
	
	public Vector3[] vertexes;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	// Draw the spline path as a gizmo
	void OnDrawGizmos() {
		Gizmos.color = new Color (0.7f, 1.0f, 0.7f);
		for (int i = 1; i < vertexes.Length; i ++) {
			Vector3 v0 = transform.TransformPoint(vertexes[i-1]);
			Vector3 v1 = transform.TransformPoint(vertexes[i]);
			Gizmos.DrawLine (v0, v1);
			if (i == 1)
				Gizmos.DrawWireSphere(v0, 0.01f);
			Gizmos.DrawWireSphere(v1, 0.01f);
		}
	}
}
