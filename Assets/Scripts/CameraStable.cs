using UnityEngine;
using System.Collections;

public class CameraStable : MonoBehaviour {

	public Transform target;
	public float smoothing = 5f;
	private Vector3 moveVector;
	
	Vector3 offset;
	
	void Start() {
		offset = transform.position - target.position;
		
	}

	void Update() {
		transform.position = Vector3.Lerp (transform.position, moveVector, smoothing * Time.deltaTime);
	}
	
	void FixedUpdate() {
		moveVector = target.position + offset;
		
		// Restrict Movement of Camera
		moveVector.x = 0;
		moveVector.y = Mathf.Clamp(moveVector.y, 3, 5);
		
		//transform.position = Vector3.Lerp(transform.position, moveVector, smoothing * Time.deltaTime);
	}
}
