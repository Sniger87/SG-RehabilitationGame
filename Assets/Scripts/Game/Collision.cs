using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Profiles;
using Configurations;

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

    private List<float[]> collisionList = new List<float[]>();

    void Start()
    {
        sourceAudio = gameObject.AddComponent<AudioSource>();
        sourceAudio.mute = ConfigManager.Current.GameConfig.EffectsMute;
        sourceAudio.volume = ConfigManager.Current.GameConfig.EffectsVolumeLevel;
        maxCollisions = SpawnManager.Instance.AmountObstacles;
        maxCoins = SpawnManager.Instance.AmountCoins;
        ChangeSuccessBar();
    }

    void OnCollisionEnter(UnityEngine.Collision col)
    {
        if (col.gameObject.tag == "Obstacle")
        {
            float[] colData = getCollisionData(col);
            collisionList.Add(colData);
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
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Finish")
        {
            SaveInfosIntoUser();
            Finish.text = string.Format("Success!\nCoins gained: {0}/{1}\nHighscore: {2}",
                coinsCounter.ToString(),
                SpawnManager.Instance.AmountCoins.ToString(),
                ProfileManager.Current.CurrentPlayer.CurrentHighscore);
            FinishPanel.SetActive(true);
            //collision data to ki
            KI.Instance.reset(collisionList);
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
            Countdown.text = string.Format("Next Round in\n{0}",
                t.ToString());
            yield return new WaitForSeconds(1.0f);
            t--;
        }
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    private float[] getCollisionData(UnityEngine.Collision col)
    {
        float[] collisionData = new float[2];
        Vector3 colVec = col.contacts[0].point;
        collisionData[0] = colVec.z;
        collisionData[1] = colVec.x;
        //Debug.Log(colVec.x);
        return collisionData;
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

            float weightCoins = (75.0f);
            int chanceEmpty = KI.Instance.calculateEmpty(SpawnManager.Instance.AmountObstacles);
            if (chanceEmpty == 0)
            {
                chanceEmpty = 1;
            }
            float weightCollisions = (25.0f * chanceEmpty);

            currentPlayer.Coins = coinsCounter;
            currentPlayer.Collisions = collisionsCounter;

            currentPlayer.CurrentHighscore += (int)(-(weightCollisions * currentPlayer.Collisions)
                + (weightCoins * currentPlayer.Coins));
        }
    }
}
