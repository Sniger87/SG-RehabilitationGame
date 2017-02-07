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
    public AudioSource MenuAudioSource;

    // Use this for initialization
    void Start()
    {
        // Wichtig! Reigenfolge muss zur initialisierung eingehalten werden
        ConfigManager.Current.Initialize();
        ProfileManager.Current.LoadPlayers();

        ProfileManager.Current.CurrentPlayer = ProfileManager.Current.Players.FirstOrDefault();

        MenuAudioSource = GetComponent<AudioSource>();
        if (MenuAudioSource.isActiveAndEnabled)
        {
            MenuAudioSource.tag = "MenuAudioSource";
            MenuAudioSource.mute = ConfigManager.Current.GameConfig.Mute;
            MenuAudioSource.volume = ConfigManager.Current.GameConfig.VolumeLevel;
            DontDestroyOnLoad(MenuAudioSource);
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
}
