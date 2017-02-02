using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wii.Contracts;
using Wii.Controllers;
using Wii.DesktopFacades;
using Wii.Exceptions;

public class BalanceBoardInstance : MonoBehaviour
{
    public BalanceBoard BalanceBoard;


    // Use this for initialization
    void Start()
    {
        // make BalanceBoardInstance persistent
        DontDestroyOnLoad(this);

        try
        {
            this.BalanceBoard = WiiInputManager.Current.FindWiiController(ControllerType.WiiBalanceBoard) as BalanceBoard;
        }
        catch (WiiControllerNotFoundException)
        {
            // Kein Controller angeschlossen
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.BalanceBoard != null)
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
}
