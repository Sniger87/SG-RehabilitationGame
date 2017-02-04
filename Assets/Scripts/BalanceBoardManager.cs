using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Wii.Contracts;
using Wii.Controllers;
using Wii.DesktopFacades;
using Wii.Events;
using Wii.Exceptions;

public class BalanceBoardManager
{
    #region Felder
    private static BalanceBoardManager current;
    private BalanceBoard balanceBoard;

    /// <summary>
    /// Event raised when BalanceBoard state is changed
    /// </summary>
    public event EventHandler<BalanceBoardConnectionChangedEventArgs> BalanceBoardConnectionChanged;
    #endregion

    #region Eigenschaften
    public static BalanceBoardManager Current
    {
        get
        {
            if (current == null)
            {
                current = new BalanceBoardManager();
            }
            return current;
        }
    }

    public BalanceBoard BalanceBoard
    {
        get
        {
            return this.balanceBoard;
        }
        private set
        {
            if (value != this.balanceBoard)
            {
                this.balanceBoard = value;
            }
        }
    }

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
    #endregion

    #region Konstruktor
    private BalanceBoardManager()
    {
    }
    #endregion

    #region Implementierungen
    public void Connect()
    {
        Thread t = new Thread(CreateAndConnect);
        t.Start();
    }

    private void CreateAndConnect()
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

        if (BalanceBoardConnectionChanged != null)
        {
            BalanceBoardConnectionChanged(this, new BalanceBoardConnectionChangedEventArgs(this.BalanceBoard));
        }
    }

    public void Disconnect()
    {
        Thread t = new Thread(DisconnectAndDestroy);
        t.Start();
    }

    public void DisconnectAndDestroy()
    {
        // Wichtig!! Objekt zerstören, damit Zugriff auf Speicher wieder freigegeben wird!!
        if (this.BalanceBoard != null)
        {
            this.BalanceBoard.Disconnect();
            this.BalanceBoard.Dispose();
            this.BalanceBoard = null;
        }

        if (BalanceBoardConnectionChanged != null)
        {
            BalanceBoardConnectionChanged(this, new BalanceBoardConnectionChangedEventArgs(this.BalanceBoard));
        }
    }
    #endregion
}

public class BalanceBoardConnectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// The current state of the BalanceBoard
    /// </summary>
    public BalanceBoard BalanceBoard;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="balanceBoard">BalanceBoard state</param>
    public BalanceBoardConnectionChangedEventArgs(BalanceBoard balanceBoard)
    {
        this.BalanceBoard = balanceBoard;
    }
}
