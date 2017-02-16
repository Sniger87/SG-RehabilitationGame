using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Profiles;

public class Collision : MonoBehaviour {

	public Text coinsTextCounter;
	public Text collisionsTextCounter;
	public Text finish;
	public Text countdown;

	public GameObject FinishPanel;

	public AudioClip coinAudioFX;
	public AudioClip obstacleAudioFX;

	private int coinsCounter = 0;
	private int collisionsCounter = 0;
	private AudioSource sourceAudio = null;
	private int maxCollisions; // wird aus dem Log herausgelesen, welches sukzessive aufgebaut wird - also dynamisch waechst
	private int maxCoins;
	public Slider successBar;

	void Start() {
		sourceAudio = gameObject.AddComponent<AudioSource>();
		maxCollisions = 20;
		maxCoins = 5;
		changeSuccessBar ();
	}

	private void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == "Obstacle") {
			Destroy (col.gameObject);
			collisionsCounter++;
			SetText ();
			PlayFX (obstacleAudioFX);
			changeSuccessBar ();
		}
		if (col.gameObject.tag == "Coin") {
			Destroy (col.gameObject);
			coinsCounter++;
			SetText ();
			PlayFX (coinAudioFX);
			changeSuccessBar ();
		}
		if (col.gameObject.tag == "Finish") {
			finish.text = "Geschafft! \n\n";
			finish.text += "Eingesammelte Münzen:  " + coinsCounter.ToString () + "\n";
			finish.text += "Highscore, Zeit, etc...";
			FinishPanel.SetActive(true);
			saveInfosIntoUser();
			//wait 5 seconds
			StartCoroutine(Wait());
		}
	}

	private void SetText () {
		coinsTextCounter.text = "Coins: " + coinsCounter.ToString ();
		collisionsTextCounter.text = "Collisions: " + collisionsCounter.ToString ();
	}

	private void PlayFX(AudioClip clip) {
		sourceAudio.clip = clip;
		sourceAudio.Play ();
	}

	private IEnumerator Wait() {
		int t = 5;
		while (t > 0) {
			countdown.text = t.ToString();
			yield return new WaitForSeconds (1.5f);
			t--;
		}
		SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
	}

	private void changeSuccessBar() {
		// cast ist notwendig und wird mittels der Multiplikation 1f gemacht
		successBar.value = 1-2*((collisionsCounter*1f)/maxCollisions)+((coinsCounter*1f)/maxCoins);
	}

	private void saveInfosIntoUser() {
		int qtyElements = 3;
		float weightLevel = float.Parse("" + (2.0/qtyElements));
		float weightCoins = float.Parse("" + (0.25/qtyElements));
		float weightCollisions = float.Parse("" + (0.75/qtyElements));

		Profiles.ProfileManager.Current.CurrentPlayer.Level += 1;

		Profiles.ProfileManager.Current.CurrentPlayer.Highscores.Add(int.Parse("" +
		(weightLevel * Profiles.ProfileManager.Current.CurrentPlayer.Level
		- weightCollisions * Profiles.ProfileManager.Current.CurrentPlayer.Collisions
		+ weightCoins * Profiles.ProfileManager.Current.CurrentPlayer.Coins)));
	}
}
