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
    #region Felder
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

    private CultureInfo currentCulture;
    #endregion

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
                BalanceBoardManager.Current.GetUpdate();

                if (weightUnitLb)
                {
                    this.TopLeft.text = BalanceBoardManager.Current.BalanceBoardState.SensorValuesLb.TopLeft.ToString("F4", currentCulture);
                    this.TopRight.text = BalanceBoardManager.Current.BalanceBoardState.SensorValuesLb.TopRight.ToString("F4", currentCulture);
                    this.BottomLeft.text = BalanceBoardManager.Current.BalanceBoardState.SensorValuesLb.BottomLeft.ToString("F4", currentCulture);
                    this.BottomRight.text = BalanceBoardManager.Current.BalanceBoardState.SensorValuesLb.BottomRight.ToString("F4", currentCulture);
                    this.Weight.text = BalanceBoardManager.Current.BalanceBoardState.WeightLb.ToString("F4", currentCulture);
                }
                else
                {
                    this.TopLeft.text = BalanceBoardManager.Current.BalanceBoardState.SensorValuesKg.TopLeft.ToString("F4", currentCulture);
                    this.TopRight.text = BalanceBoardManager.Current.BalanceBoardState.SensorValuesKg.TopRight.ToString("F4", currentCulture);
                    this.BottomLeft.text = BalanceBoardManager.Current.BalanceBoardState.SensorValuesKg.BottomLeft.ToString("F4", currentCulture);
                    this.BottomRight.text = BalanceBoardManager.Current.BalanceBoardState.SensorValuesKg.BottomRight.ToString("F4", currentCulture);
                    this.Weight.text = BalanceBoardManager.Current.BalanceBoardState.WeightKg.ToString("F4", currentCulture);
                }

                if (isCalibrating)
                {
                    if (isCapturingMin)
                    {
                        BalanceBoardManager.Current.CaptureMin();
                    }
                    if (isCapturingMax)
                    {
                        BalanceBoardManager.Current.CaptureMax();
                    }
                }
            }
        }

        if (isCalibratingFinished)
        {
            isCalibratingFinished = false;
            BalanceBoardManager.Current.CaptureFinished();
        }

        if (BalanceBoardManager.Current.IsBalanceBoardConnected && !isCalibrating && !isCalibratingFinished)
        {
            float x = BalanceBoardManager.Current.BalanceBoardState.CenterOfGravityMax.X - BalanceBoardManager.Current.BalanceBoardState.CenterOfGravity.X;
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
