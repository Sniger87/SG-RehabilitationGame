using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Configurations;
using UnityEngine;
using Wii.Contracts;
using Wii.Controllers;
using Wii.DesktopFacades;
using Wii.Events;
using Wii.Exceptions;

public class BalanceBoardManager
{
    #region Konstanten
    // These are the Win32 error code for file not found or access denied.
    private const int ERROR_FILE_NOT_FOUND = 2;
    private const int ERROR_ACCESS_DENIED = 5;
    private const string ServiceFolder = "WiiConsoleService";
    private const string ServiceProgram = "WiiConsoleService.exe";
    private const string CalibrationFileName = "BalanceBoardCalibration.json";
    #endregion

    #region Felder
    private static BalanceBoardManager current;
    private BalanceBoard balanceBoard;
    private Process balanceBoardProcess;
    private string calibrationFilePath;
    private string exePath;

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
            if (this.BalanceBoard != null && this.BalanceBoard.IsConnected && this.BalanceBoard.IsInitialized)
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
        this.exePath = Path.Combine(Application.dataPath, ServiceFolder);
        this.exePath = Path.Combine(exePath, ServiceProgram);
        this.calibrationFilePath = Path.Combine(ConfigManager.Current.GameDirectoryPath, CalibrationFileName);
    }
    #endregion

    #region Implementierungen
    public void Connect()
    {
        Thread t = new Thread(new ParameterizedThreadStart(CreateAndConnect));
        t.Start(this.exePath);
    }

    private void CreateAndConnect(object path)
    {
        try
        {
            if (balanceBoardProcess != null)
            {
                balanceBoardProcess.Kill();
                balanceBoardProcess.Dispose();
                balanceBoardProcess = null;
            }

            balanceBoardProcess = new Process();
            balanceBoardProcess.StartInfo.FileName = path as string;
            balanceBoardProcess.StartInfo.Arguments = this.calibrationFilePath;
            balanceBoardProcess.StartInfo.CreateNoWindow = true;
            balanceBoardProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            balanceBoardProcess.EnableRaisingEvents = true;
            balanceBoardProcess.Exited += BalanceBoardProcess_Exited;
            balanceBoardProcess.Start();
        }
        catch (Win32Exception e)
        {
            if (e.NativeErrorCode == ERROR_FILE_NOT_FOUND)
            {
                Log(e.Message + ". Check the path.");
            }
            else if (e.NativeErrorCode == ERROR_ACCESS_DENIED)
            {
                Log(e.Message + ". You do not have permission to open this file.");
            }
            else
            {
                Log(e.Message);
            }

            // signal program to cancel connecting
            if (BalanceBoardConnectionChanged != null)
            {
                BalanceBoardConnectionChanged(this, new BalanceBoardConnectionChangedEventArgs(this.BalanceBoard));
            }
        }
    }

    private void BalanceBoardProcess_Exited(object sender, EventArgs e)
    {
        Process process = sender as Process;
        if (process != null)
        {
            if (process.ExitCode == 0)
            {
                // Erfolg! Calibration lesen
                if (File.Exists(this.calibrationFilePath))
                {
                    using (StreamReader reader = File.OpenText(this.calibrationFilePath))
                    {
                        string line = reader.ReadToEnd();
                        try
                        {
                            if (this.BalanceBoard == null)
                            {
                                this.BalanceBoard = WiiInputManager.Current.FindWiiController(ControllerType.WiiBalanceBoard) as BalanceBoard;
                            }
                            this.BalanceBoard.Connect(JsonUtility.FromJson<BalanceBoardCalibrationInfo>(line));
                        }
                        catch (WiiControllerNotFoundException ex)
                        {
                            // Kein Controller angeschlossen
                            Log(ex.Message);
                        }
                    }
                }
            }
        }

        if (BalanceBoardConnectionChanged != null)
        {
            BalanceBoardConnectionChanged(this, new BalanceBoardConnectionChangedEventArgs(this.BalanceBoard));
        }
    }

    public void Disconnect(bool destroy = false)
    {
        Thread t = new Thread(DisconnectAndDestroy);
        t.Start(destroy);
    }

    public void DisconnectAndDestroy(object value)
    {
        bool destroy = (bool)value;

        if (balanceBoardProcess != null)
        {
            if (!balanceBoardProcess.HasExited)
            {
                balanceBoardProcess.Kill();
            }
            balanceBoardProcess.Dispose();
            balanceBoardProcess = null;
        }

        // Wichtig!! Objekt zerstören, damit Zugriff auf Speicher wieder freigegeben wird!!
        if (this.BalanceBoard != null)
        {
            this.BalanceBoard.Disconnect();
            if (destroy)
            {
                this.BalanceBoard.Dispose();
                this.BalanceBoard = null;
            }
        }

        if (BalanceBoardConnectionChanged != null)
        {
            BalanceBoardConnectionChanged(this, new BalanceBoardConnectionChangedEventArgs(this.BalanceBoard));
        }
    }

    protected void Log(string content)
    {
        string path = Configurations.ConfigManager.Current.GameLogPath;
        FileIO.FileManager.AppendLog(path, content);
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
