using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wii.Contracts;
using Wii.Controllers;
using Wii.DesktopFacades;
using Wii.Exceptions;

public class BalanceBoardInstance : MonoBehaviour
{
    public BalanceBoard BalanceBoard;
    public static BalanceBoardInstance Instance;

    public bool IsBalanceBoardConnected
    {
        get
        {
            if (this.BalanceBoard != null && this.BalanceBoard.IsConnected)
            {
                return true;
            }
            return false;
        }
    }

    // Use this for initialization
    void Start()
    {

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
        if (this.IsBalanceBoardConnected)
        {
            this.BalanceBoard.GetUpdate();
        }
    }

    private void OnDestroy()
    {
        // Wichtig!! Objekt zerstören, damit Zugriff auf Speicher wieder freigegeben wird!!
        if (this.BalanceBoard != null)
        {
            this.BalanceBoard.Disconnect();
            this.BalanceBoard.Dispose();
            this.BalanceBoard = null;
        }
    }

    public void Connect()
    {
        try
        {
            if (this.BalanceBoard == null)
            {
                this.BalanceBoard = WiiInputManager.Current.FindWiiController(ControllerType.WiiBalanceBoard) as BalanceBoard;
            }
            else
            {
                this.BalanceBoard.Connect();
            }
        }
        catch (WiiControllerNotFoundException)
        {
            // Kein Controller angeschlossen
        }
    }
}
