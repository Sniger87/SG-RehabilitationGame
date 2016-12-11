using UnityEngine;
using System.Collections;

public class Sync2Player : MonoBehaviour {
	private Rigidbody rigidbodyComponent;
	private Vector3 movement;
	private Vector3 buffer;

		// Use this for initialization
		void Start () {
			rigidbodyComponent = GetComponent<Rigidbody>();
			buffer = new Vector3 (0, 0, 0);
			if (gameObject.GetComponent<Player> () != null) {
				Player player = gameObject.GetComponent<Player> ();
				buffer = player.getMovement ();
			}
		}

		// Update is called once per frame
		void Update() {
			if (gameObject.GetComponent<Player> () != null) {
				Player player = gameObject.GetComponent<Player> ();
				buffer = player.getMovement ();
			}
			movement = buffer;
		}

		void FixedUpdate() {
		// Move the game object
		rigidbodyComponent.velocity = movement;
		}
	}


