using System.Collections;
using System.Collections.Generic;
using Profiles;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject LoadImage;
    public GameObject ImageConnected;
    public GameObject ImageDisconnected;
    public Text PlayerInfo;
    public Text BalanceBoardInfo;

    // Use this for initialization
    void Start()
    {
        if (BalanceBoardManager.Current.IsBalanceBoardConnected)
        {
            //BalanceBoardInfo.text = "BalanceBoard connected";
            ImageConnected.SetActive(true);
        }
        else
        {
            //BalanceBoardInfo.text = "BalanceBoard not connected";
            ImageDisconnected.SetActive(true);
        }

        if (ProfileManager.Current.CurrentPlayer != null)
        {
            PlayerInfo.text = string.Format("Player: {0}", ProfileManager.Current.CurrentPlayer.Name);
        }
        else
        {
            PlayerInfo.text = "Player: N/A";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowSettings()
    {
        LoadImage.SetActive(true);
        SceneManager.LoadSceneAsync("Settings", LoadSceneMode.Single);
    }

    public void StartGame()
    {
        // Load scene
        LoadImage.SetActive(true);
        GameInstance.Instance.SetGameClip();
        ProfileManager.Current.CreateFilesForGame();
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    public void ShowBalanceBoard()
    {
        LoadImage.SetActive(true);
        SceneManager.LoadSceneAsync("BalanceBoard", LoadSceneMode.Single);
    }

    public void ShowHighscore()
    {
        LoadImage.SetActive(true);
        SceneManager.LoadSceneAsync("Highscore", LoadSceneMode.Single);
    }

    public void Back()
    {
        LoadImage.SetActive(true);
        SceneManager.LoadSceneAsync("Login", LoadSceneMode.Single);
    }
}
