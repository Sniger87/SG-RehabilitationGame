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
    public GameObject StepdownImage;
    public GameObject StepupImage;
    public GameObject BarPanel;
    public Button ConnectButton;
    public Button DisconnectButton;
    public Button RecalibrateButton;
    public Text InfoText;
    public Text TopLeft;
    public Text TopRight;
    public Text BottomLeft;
    public Text BottomRight;
    public Text Weight;
    public Text CountdownStepUpText;
    public Text CountdownStepDownText;
    public RectTransform MovementBar;

    private float nextActionTime = 0.0f;
    private float period = 0.2f;
    private bool updateBalanceBoardState;
    private bool weightUnitLb;
    private bool isCalibrating;
    private bool isCalibratingFinished;
    private bool isCapturingMax;
    private bool isCapturingMin;
    private List<float> maxValues;
    private List<float> minValues;

    private CultureInfo currentCulture;

    // Use this for initialization
    void Start()
    {
        BalanceBoardManager.Current.BalanceBoardConnectionChanged += BalanceBoardConnectionChanged;

        if (BalanceBoardManager.Current.IsBalanceBoardConnected)
        {
            SetConnectedState(false);
        }
        else
        {
            SetDisconnectedState();
        }

        currentCulture = CultureInfo.CurrentCulture;
        weightUnitLb = ConfigManager.Current.GameConfig.WeightUnitLb;

        maxValues = new List<float>();
        minValues = new List<float>();
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
                SetConnectedState(true);
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
        RecalibrateButton.interactable = false;
        InfoText.text = "disconnect";
        BalanceBoardConnectedImage.SetActive(false);
        BalanceBoardDisconnectedImage.SetActive(true);
        StatusPanel.SetActive(false);
        BarPanel.SetActive(false);
    }

    private void SetConnectedState(bool calibrate)
    {
        ConnectButton.interactable = false;
        DisconnectButton.interactable = true;
        RecalibrateButton.interactable = true;
        InfoText.text = "connect";
        BalanceBoardConnectedImage.SetActive(true);
        BalanceBoardDisconnectedImage.SetActive(false);
        StatusPanel.SetActive(true);
        BarPanel.SetActive(true);
        if (calibrate)
        {
            // get X value for move
            StartCoroutine(Calibrate());
        }
    }

    private IEnumerator Calibrate()
    {
        isCalibrating = true;
        StepdownImage.SetActive(true);
        isCapturingMin = true;
        int t = 10;
        while (t > 0)
        {
            CountdownStepDownText.text = string.Format("{0}",
                t.ToString());
            yield return new WaitForSeconds(1.0f);
            t--;
        }
        isCapturingMin = false;
        StepdownImage.SetActive(false);
        StepupImage.SetActive(true);
        isCapturingMax = true;
        t = 10;
        while (t > 0)
        {
            CountdownStepUpText.text = string.Format("{0}",
                t.ToString());
            yield return new WaitForSeconds(1.0f);
            t--;
        }
        isCapturingMax = false;
        StepupImage.SetActive(false);
        isCalibrating = false;
        isCalibratingFinished = true;
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
                BalanceBoard balanceBoard = BalanceBoardManager.Current.BalanceBoard;
                balanceBoard.GetUpdate();

                if (weightUnitLb)
                {
                    this.TopLeft.text = balanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.TopLeft.ToString("F4", currentCulture);
                    this.TopRight.text = balanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.TopRight.ToString("F4", currentCulture);
                    this.BottomLeft.text = balanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.BottomLeft.ToString("F4", currentCulture);
                    this.BottomRight.text = balanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.BottomRight.ToString("F4", currentCulture);
                    this.Weight.text = balanceBoard.WiiControllerState.BalanceBoardState.WeightLb.ToString("F4", currentCulture);
                }
                else
                {
                    this.TopLeft.text = balanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.TopLeft.ToString("F4", currentCulture);
                    this.TopRight.text = balanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.TopRight.ToString("F4", currentCulture);
                    this.BottomLeft.text = balanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomLeft.ToString("F4", currentCulture);
                    this.BottomRight.text = balanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomRight.ToString("F4", currentCulture);
                    this.Weight.text = balanceBoard.WiiControllerState.BalanceBoardState.WeightKg.ToString("F4", currentCulture);
                }

                if (isCalibrating)
                {
                    if (isCapturingMin)
                    {
                        minValues.Add(balanceBoard.WiiControllerState.BalanceBoardState.CenterOfGravity.X);
                    }
                    if (isCapturingMax)
                    {
                        maxValues.Add(balanceBoard.WiiControllerState.BalanceBoardState.CenterOfGravity.X);
                    }
                }
            }
        }

        if (isCalibratingFinished)
        {
            isCalibratingFinished = false;
            BalanceBoardManager.Current.XMax = maxValues.Average();
            BalanceBoardManager.Current.XMin = minValues.Average();
        }

        if (BalanceBoardManager.Current.IsBalanceBoardConnected && !isCalibrating && !isCalibratingFinished)
        {
            float x = BalanceBoardManager.Current.XMax - BalanceBoardManager.Current.BalanceBoard.WiiControllerState.BalanceBoardState.CenterOfGravity.X;
            MovementBar.transform.localPosition = new Vector3(x, 0, 0);
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

    public void Recalibrate()
    {
        // get X value for move
        StartCoroutine(Calibrate());
    }

    public void Back()
    {
        LoadImage.SetActive(true);
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }
}
