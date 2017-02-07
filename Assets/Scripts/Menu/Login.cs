using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Profiles;
using System.Linq;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public Dropdown DropDownPlayers;
    public GameObject LoadImage;
    public Button ButtonSelect;
    public Button ButtonDelete;

    // Use this for initialization
    void Start()
    {
        LoadDropDown();
    }

    private void LoadDropDown()
    {
        DropDownPlayers.options.Clear();
        foreach (Profiles.Player player in ProfileManager.Current.Players)
        {
            DropDownPlayers.options.Add(new Dropdown.OptionData(player.Name));
        }
        DropDownPlayers.value = -1;

        if (!DropDownPlayers.options.Any())
        {
            if (DropDownPlayers.captionText != null)
            {
                DropDownPlayers.captionText.text = "Create Player...";
            }

            ProfileManager.Current.CurrentPlayer = null;

            ButtonSelect.interactable = false;
            ButtonDelete.interactable = false;
        }
        else
        {
            ButtonSelect.interactable = true;
            ButtonDelete.interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnValueChanged()
    {
        int value = DropDownPlayers.value;
        if (value < 0 && value < ProfileManager.Current.Players.Count)
        {
            return;
        }
        ProfileManager.Current.CurrentPlayer = ProfileManager.Current.Players.ElementAtOrDefault(value);
    }

    public void SelectPlayer()
    {
        if (ProfileManager.Current.CurrentPlayer != null)
        {
            LoadImage.SetActive(true);
            SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
        }
    }

    public void CreatePlayer()
    {
        LoadImage.SetActive(true);
        SceneManager.LoadSceneAsync("CreatePlayer", LoadSceneMode.Single);
    }

    public void DeletePlayer()
    {
        if (ProfileManager.Current.CurrentPlayer != null)
        {
            ProfileManager.Current.DeletePlayer(ProfileManager.Current.CurrentPlayer);
            LoadDropDown();
        }
    }

    public void Close()
    {
        Application.Quit();
    }
}
