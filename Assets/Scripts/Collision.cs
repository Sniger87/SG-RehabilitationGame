using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collision : MonoBehaviour {

	public Text counter;
	public Text collisions;
	public AudioClip coinFX;
	public AudioClip obstacleFX;

	private int coinsCollected = 0;
	private int collisionCounter = 0;
	private AudioSource source = null;

	void Start() {
		source = gameObject.AddComponent<AudioSource>(); 
	}

	void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == "Obstacle") {
			collisionCounter++;
			Destroy (col.gameObject);
			SetText ();
			PlayFX (obstacleFX);
		}

		if (col.gameObject.tag == "Coin") {

			coinsCollected++;
			Destroy (col.gameObject);
			SetText ();
			PlayFX (coinFX);
		}
	}

	void SetText () {
		counter.text = "Coins: " + coinsCollected.ToString ();
		collisions.text = "Collisions: " + collisionCounter.ToString ();
	}
	void PlayFX(AudioClip clip) {
		source.clip = clip;
		source.Play ();
	}
}
