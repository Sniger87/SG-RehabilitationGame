using UnityEngine;
using System.Collections;

// Player controller and behavior
public class Player : MonoBehaviour {

	public Vector3 speed;
	private Vector3 movement;
	private Rigidbody rigidbodyComponent;

	public Vector3 getSpeed() {
		return speed;
	}

	public Vector3 getMovement() {
		return movement;
	}

	// Use this for initialization
	void Start () {
		speed = new Vector3(2, 2, 2);
		rigidbodyComponent = GetComponent<Rigidbody>();
		movement = new Vector3 (0, 0, 0);
	}

	// Update is called once per frame
	void Update() {
		// Retrieve axis information
		float inputX = Input.GetAxis("Horizontal");
		float inputY = 0;
		float inputZ = Input.GetAxis("Vertical");

		if (inputZ == 0)
			inputZ = 1;
		else if (inputZ < 0)
			inputZ = 0.1f;

		// Movement per direction
		movement.x = speed.x * inputX * Time.deltaTime;
		movement.y = speed.y * inputY * Time.deltaTime;
		movement.z = speed.z * inputZ * Time.deltaTime;
	}

	void FixedUpdate() {
		// Get the component and store the reference
		// Move the game object
		rigidbodyComponent.velocity = movement;
	}
}


