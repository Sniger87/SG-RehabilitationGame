using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wii.Exceptions;

public class BalanceBoardMenuScript : MonoBehaviour
{
    public GameObject LoadImage;
    public GameObject ConnectingImage;
    public GameObject DisconnectingImage;
    public Text InfoText;
    public Button ConnectButton;
    public Button DisconnectButton;

    // Use this for initialization
    void Start()
    {
        if (BalanceBoardManager.Current.IsBalanceBoardConnected)
        {
            ConnectButton.interactable = false;
            DisconnectButton.interactable = true;
            InfoText.text = "BalanceBoard connect";
        }
        else
        {
            ConnectButton.interactable = true;
            DisconnectButton.interactable = false;
            InfoText.text = "BalanceBoard disconnect";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Connect()
    {
        ConnectingImage.SetActive(true);

        BalanceBoardManager.Current.Connect();

        if (BalanceBoardManager.Current.IsBalanceBoardConnected)
        {
            ConnectButton.interactable = false;
            DisconnectButton.interactable = true;
            InfoText.text = "BalanceBoard found";
        }
        else
        {
            InfoText.text = "BalanceBoard not found";
        }
        ConnectingImage.SetActive(false);
    }

    public void Disconnect()
    {
        DisconnectingImage.SetActive(true);

        BalanceBoardManager.Current.Disconnect();

        ConnectButton.interactable = true;
        DisconnectButton.interactable = false;

        InfoText.text = "BalanceBoard disconnect";

        DisconnectingImage.SetActive(false);
    }

    public void Back()
    {
        LoadImage.SetActive(true);
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }
}
