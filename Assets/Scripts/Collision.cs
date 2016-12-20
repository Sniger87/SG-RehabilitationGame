using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collision : MonoBehaviour {

	public Text counter;
	public Text collisions;

	private int coinsCollected = 0;
	private int collisionCounter = 0;

	void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == "Obstacle") {
			collisionCounter++;
			Destroy (col.gameObject);
			SetText ();
		}

		if (col.gameObject.tag == "Coin") {

			coinsCollected++;
			Destroy (col.gameObject);
			SetText ();
		}
	}

	void SetText () {
		counter.text = "Coins: " + coinsCollected.ToString ();
		collisions.text = "Collisions: " + collisionCounter.ToString ();
	}
}
