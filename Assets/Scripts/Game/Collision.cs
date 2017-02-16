using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Profiles;

public class Collision : MonoBehaviour
{
    public Text CoinsTextCounter;
    public Text CollisionsTextCounter;
    public Text Finish;
    public Text Countdown;

    public GameObject FinishPanel;

    public AudioClip CoinAudioFX;
    public AudioClip ObstacleAudioFX;

    public Slider SuccessBar;

    private int coinsCounter = 0;
    private int collisionsCounter = 0;
    private AudioSource sourceAudio = null;
    private int maxCollisions;
    private int maxCoins;

    void Start()
    {
        sourceAudio = gameObject.AddComponent<AudioSource>();
        maxCollisions = SpawnManager.Instance.AmountObstacles;
        maxCoins = SpawnManager.Instance.AmountCoins;
        ChangeSuccessBar();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Obstacle")
        {
            Destroy(col.gameObject);
            collisionsCounter++;
            SetText();
            PlayFX(ObstacleAudioFX);
            ChangeSuccessBar();
        }
        if (col.gameObject.tag == "Coin")
        {
            Destroy(col.gameObject);
            coinsCounter++;
            SetText();
            PlayFX(CoinAudioFX);
            ChangeSuccessBar();
        }
        if (col.gameObject.tag == "Finish")
        {
            Finish.text = "Geschafft! \n\n";
            Finish.text += "Eingesammelte Münzen:  " + coinsCounter.ToString() + "\n";
            Finish.text += "Highscore, Zeit, etc...";
            FinishPanel.SetActive(true);
            SaveInfosIntoUser();
            //wait 5 seconds
            StartCoroutine(Wait());
        }
    }

    private void SetText()
    {
        CoinsTextCounter.text = "Coins: " + coinsCounter.ToString();
        CollisionsTextCounter.text = "Collisions: " + collisionsCounter.ToString();
    }

    private void PlayFX(AudioClip clip)
    {
        sourceAudio.clip = clip;
        sourceAudio.Play();
    }

    private IEnumerator Wait()
    {
        int t = 5;
        while (t > 0)
        {
            Countdown.text = t.ToString();
            yield return new WaitForSeconds(1.5f);
            t--;
        }
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    private void ChangeSuccessBar()
    {
        if (SuccessBar != null)
        {
            // cast ist notwendig und wird mittels der Multiplikation 1f gemacht
            SuccessBar.value = 0.5f - ((float)collisionsCounter / (float)maxCollisions) + ((float)coinsCounter / (float)maxCoins);
        }
    }

    private void SaveInfosIntoUser()
    {
        if (ProfileManager.Current.CurrentPlayer != null)
        {
            Profiles.Player currentPlayer = ProfileManager.Current.CurrentPlayer;

            float qtyElements = 3.0f;
            float weightLevel = (2.0f / qtyElements);
            float weightCoins = (0.25f / qtyElements);
            float weightCollisions = (0.75f / qtyElements);

            currentPlayer.Level += 1;
            currentPlayer.Coins = coinsCounter;
            currentPlayer.Collisions = collisionsCounter;

            int highscore = (int)((weightLevel * currentPlayer.Level)
                - (weightCollisions * currentPlayer.Collisions)
                + (weightCoins * currentPlayer.Coins));

            currentPlayer.Highscores.Add(highscore);
        }
    }
}
