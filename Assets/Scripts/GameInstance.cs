using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configurations;
using Profiles;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        // make GameInstance persistent
        DontDestroyOnLoad(this);

        // Wichtig! Reigenfolge muss zur initialisierung eingehalten werden
        ConfigManager.Current.LoadGameConfig();
        ProfileManager.Current.LoadPlayers();

        // TODO: nachher wieder rausnehmen, nur um Test Profil zu haben
        Profiles.Player currentPlayer = null;
        if (!ProfileManager.Current.Players.Any(p => p.Name == "Default"))
        {
            currentPlayer = ProfileManager.Current.CreatePlayer("Default");
        }
        else
        {
            currentPlayer = ProfileManager.Current.Players.FirstOrDefault();
        }
        ProfileManager.Current.CurrentPlayer = currentPlayer;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        // Wichtig! Reigenfolge muss eingehalten werden
        ProfileManager.Current.SavePlayers();
        ConfigManager.Current.SaveGameConfig();
    }
}
