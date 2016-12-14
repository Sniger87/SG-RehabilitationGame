using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour {

	void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == "Obstacle") {
			
			Destroy (col.gameObject);

		}

		if (col.gameObject.tag == "Coin") {

			Destroy (col.gameObject);
		}
	}
}
