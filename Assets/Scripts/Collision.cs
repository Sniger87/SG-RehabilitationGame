using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collision : MonoBehaviour {

	public Text counter;
	public Text collisions;
	public Text finish;

	public AudioClip coinAudioFX;
	public AudioClip obstacleAudioFX;
	public AudioClip coinVisualFX;
	public AudioClip obstacleVisualFX;

	private int coinsCollected = 0;
	private int collisionCounter = 0;
	private AudioSource sourceAudio = null;
	private ParticleSystem sourceVisual = null;

	void Start() {
		sourceAudio = gameObject.AddComponent<AudioSource>(); 
		sourceVisual = gameObject.AddComponent<ParticleSystem> ();
		//sourceVisual.main.loop = false;
	}

	void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == "Obstacle") {
			Destroy (col.gameObject);
			collisionCounter++;
			SetText ();
			PlayFX (obstacleAudioFX);
			VisualizeFX (obstacleVisualFX);
		}

		if (col.gameObject.tag == "Coin") {
			Destroy (col.gameObject);
			coinsCollected++;
			SetText ();
			PlayFX (coinAudioFX);
			VisualizeFX (coinVisualFX);
		}

		if (col.gameObject.tag == "Finish") {
			Debug.Log ("Finished~");
			//finish.text = "Fertig!  " + "Eingesammelte Münzen: " + coinsCollected.ToString ();
			//finish.enabled = true;
			//start transition sequence & load new level
		}
	}

	void SetText () {
		counter.text = "Coins: " + coinsCollected.ToString ();
		collisions.text = "Collisions: " + collisionCounter.ToString ();
	}
	void PlayFX(AudioClip clip) {
		sourceAudio.clip = clip;
		sourceAudio.Play ();
	}
	void VisualizeFX (AudioClip clip) {
		//sourceVisual. = clip;
		//sourceVisual.Play;
	}
}
