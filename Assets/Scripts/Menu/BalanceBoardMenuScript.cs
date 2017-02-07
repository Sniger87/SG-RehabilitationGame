using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wii.Controllers;

public class BalanceBoardMenuScript : MonoBehaviour
{
    public GameObject LoadImage;
    public GameObject ConnectingImage;
    public GameObject DisconnectingImage;
    public GameObject BalanceBoardConnectedImage;
    public GameObject BalanceBoardDisconnectedImage;
    public Text InfoText;
    public Button ConnectButton;
    public Button DisconnectButton;

    private bool updateBalanceBoardState;

    // Use this for initialization
    void Start()
    {
        BalanceBoardManager.Current.BalanceBoardConnectionChanged += BalanceBoardConnectionChanged;

        if (BalanceBoardManager.Current.IsBalanceBoardConnected)
        {
            SetConnectedState();
        }
        else
        {
            SetDisconnectedState();
        }
    }

    private void BalanceBoardConnectionChanged(object sender, BalanceBoardConnectionChangedEventArgs e)
    {
        this.updateBalanceBoardState = true;
    }

    public void UpdateBalanceBoardState(BalanceBoard balanceBoard)
    {
        ConnectingImage.SetActive(false);
        DisconnectingImage.SetActive(false);

        if (balanceBoard == null)
        {
            SetDisconnectedState();
            InfoText.text = "not found";
        }
        else
        {
            if (balanceBoard.IsConnected && balanceBoard.IsInitialized)
            {
                SetConnectedState();
            }
            else
            {
                SetDisconnectedState();
                // Lieber wieder Verbindung trennen wenn initialisierung nicht erfolgreich war
                balanceBoard.Disconnect();
            }
        }
        this.updateBalanceBoardState = false;
    }

    private void SetDisconnectedState()
    {
        ConnectButton.interactable = true;
        DisconnectButton.interactable = false;
        InfoText.text = "disconnect";
        BalanceBoardConnectedImage.SetActive(false);
        BalanceBoardDisconnectedImage.SetActive(true);
    }

    private void SetConnectedState()
    {
        ConnectButton.interactable = false;
        DisconnectButton.interactable = true;
        InfoText.text = "connect";
        BalanceBoardConnectedImage.SetActive(true);
        BalanceBoardDisconnectedImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.updateBalanceBoardState)
        {
            UpdateBalanceBoardState(BalanceBoardManager.Current.BalanceBoard);
        }
    }

    private void OnDestroy()
    {
        BalanceBoardManager.Current.BalanceBoardConnectionChanged -= BalanceBoardConnectionChanged;
    }

    public void Connect()
    {
        ConnectingImage.SetActive(true);

        BalanceBoardManager.Current.Connect();
    }

    public void Disconnect()
    {
        DisconnectingImage.SetActive(true);

        BalanceBoardManager.Current.Disconnect();
    }

    public void Back()
    {
        LoadImage.SetActive(true);
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }
}
