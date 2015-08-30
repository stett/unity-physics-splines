using UnityEngine;
using System.Collections;

public class InitialVelocity : MonoBehaviour {

	public Vector3 velocity;

	// Use this for initialization
	void Start () {
		Rigidbody body = GetComponent<Rigidbody> ();
		body.velocity = velocity;
	}
}
