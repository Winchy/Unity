using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : MonoBehaviour {

	public float moveSpeed;

	public float acceleration;

	public float minDist;

	public float maxDist;

	public float avoidDist;

	public float arrivalDist;

	public float checkInterval;

	private ArrayList points;

	private Vector3 velocity;

	private Vector3 currentPosition;

	//current location
	private Vector3 nextPoint;

	// Use this for initialization
	void Start () {
		//Random.InitState (System.DateTime.Now.Millisecond);
		currentPosition = transform.position;
		nextPoint = transform.position;
		velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		currentPosition = transform.position;

		if (Vector3.Distance (currentPosition, nextPoint) <= arrivalDist) {
			Vector3 v = Vector3.zero;
			GenerateNextPoint (v);
		} 

		CheckCollision ();

		if (velocity.magnitude > 0) {
			transform.rotation = Quaternion.LookRotation (velocity);
		}

		transform.position += velocity * Time.fixedDeltaTime;

		var force = Force ();
		velocity += force * Time.fixedDeltaTime * acceleration;
	}

	void GenerateNextPoint(Vector3 direction) {
		//Debug.Log (transform.position);
		if (direction == Vector3.zero) {
			Vector2 r = Random.insideUnitCircle;
			direction = new Vector3 (r.x, 0, r.y);
		}

		direction = direction.normalized;

		nextPoint = currentPosition + direction * Random.Range (minDist, maxDist);

		//Debug.Log (nextPoint);
	}

	void CheckCollision() {
		RaycastHit hit;
		Vector3 destPoint = currentPosition + transform.forward * avoidDist;
		if (Physics.Raycast (currentPosition, transform.forward, out hit, avoidDist)
			|| Physics.Raycast (destPoint, -transform.forward, out hit, avoidDist)) {
			var hitNormal = hit.normal;
			//Debug.Log (hitNormal);
			float d = Vector3.Dot (hitNormal, transform.forward);
			//Debug.Log (d);
			if (d > 0) {
				hitNormal = -hitNormal;
			}

			var reflectVector = 2.0f * Mathf.Abs (d) * hitNormal + transform.forward;
			reflectVector.y = 0;
			GenerateNextPoint (reflectVector);
		}

	}

	Vector3 Force() {
		var dist = nextPoint - currentPosition;
		var desiredVelocity = dist.normalized * moveSpeed;
		return desiredVelocity - velocity;
	}
}
