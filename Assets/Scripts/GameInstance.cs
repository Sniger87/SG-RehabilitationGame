using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configurations;
using Profiles;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInstance : MonoBehaviour
{
    public static GameInstance Instance;
    public AudioSource AudioSource;
    public AudioClip MenuClip;
    public AudioClip GameClip;

    // Use this for initialization
    void Start()
    {
        // Wichtig! Reigenfolge muss zur initialisierung eingehalten werden
        ConfigManager.Current.Initialize();
        ProfileManager.Current.LoadPlayers();

        ProfileManager.Current.CurrentPlayer = ProfileManager.Current.Players.FirstOrDefault();

        if (this.AudioSource == null)
        {
            this.AudioSource = GetComponent<AudioSource>();
        }
        if (this.AudioSource != null && this.AudioSource.isActiveAndEnabled)
        {
            this.AudioSource.clip = MenuClip;
            this.AudioSource.tag = "GlobalAudioSource";
            this.AudioSource.mute = ConfigManager.Current.GameConfig.MusicMute;
            this.AudioSource.volume = ConfigManager.Current.GameConfig.MusicVolumeLevel;
            this.AudioSource.Play();
            DontDestroyOnLoad(AudioSource);
        }

        // Show the first scene
        SceneManager.LoadSceneAsync("Login", LoadSceneMode.Single);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        // BalanceBoard unbedingt verwerfen!!
        BalanceBoardManager.Current.Disconnect(destroy: true);
        // Wichtig! Reigenfolge muss eingehalten werden
        ProfileManager.Current.SavePlayers();
        ConfigManager.Current.SaveGameConfig();
    }

    public void SetGameClip()
    {
        if (this.AudioSource != null)
        {
            this.AudioSource.Stop();
            this.AudioSource.clip = GameClip;
            this.AudioSource.Play();
        }
    }

    public void SetMenuClip()
    {
        if (this.AudioSource != null)
        {
            this.AudioSource.Stop();
            AudioSource.clip = MenuClip;
            this.AudioSource.Play();
        }
    }
}
