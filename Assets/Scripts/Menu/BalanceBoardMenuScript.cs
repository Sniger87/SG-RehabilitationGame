using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Configurations;
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
    public GameObject StatusPanel;
    public Text InfoText;
    public Button ConnectButton;
    public Button DisconnectButton;
    public Text TopLeft;
    public Text TopRight;
    public Text BottomLeft;
    public Text BottomRight;
    public Text Weight;

    private float nextActionTime = 0.0f;
    private float period = 0.2f;
    private bool updateBalanceBoardState;
    private bool weightUnitLb;

    private CultureInfo currentCulture;

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

        currentCulture = CultureInfo.CurrentCulture;
        weightUnitLb = ConfigManager.Current.GameConfig.WeightUnitLb;
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
        StatusPanel.SetActive(false);
    }

    private void SetConnectedState()
    {
        ConnectButton.interactable = false;
        DisconnectButton.interactable = true;
        InfoText.text = "connect";
        BalanceBoardConnectedImage.SetActive(true);
        BalanceBoardDisconnectedImage.SetActive(false);
        StatusPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.updateBalanceBoardState)
        {
            UpdateBalanceBoardState(BalanceBoardManager.Current.BalanceBoard);
        }

        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            if (BalanceBoardManager.Current.IsBalanceBoardConnected)
            {
                BalanceBoardManager.Current.BalanceBoard.GetUpdate();

                if (weightUnitLb)
                {
                    this.TopLeft.text = BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.TopLeft.ToString("F4", currentCulture);
                    this.TopRight.text = BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.TopRight.ToString("F4", currentCulture);
                    this.BottomLeft.text = BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.BottomLeft.ToString("F4", currentCulture);
                    this.BottomRight.text = BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.BottomRight.ToString("F4", currentCulture);
                    this.Weight.text = BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.WeightLb.ToString("F4", currentCulture);
                }
                else
                {
                    this.TopLeft.text = BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.TopLeft.ToString("F4", currentCulture);
                    this.TopRight.text = BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.TopRight.ToString("F4", currentCulture);
                    this.BottomLeft.text = BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomLeft.ToString("F4", currentCulture);
                    this.BottomRight.text = BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomRight.ToString("F4", currentCulture);
                    this.Weight.text = BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.WeightKg.ToString("F4", currentCulture);
                }
            }
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
