using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collision : MonoBehaviour {

	public Text counter;
	public Text collisions;
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
			collisionCounter++;
			SetText ();
			PlayFX (obstacleAudioFX);
			VisualizeFX (obstacleVisualFX);
			Destroy (col.gameObject);
		}

		if (col.gameObject.tag == "Coin") {
			coinsCollected++;
			SetText ();
			PlayFX (coinAudioFX);
			VisualizeFX (coinVisualFX);
			Destroy (col.gameObject);
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
