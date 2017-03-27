using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    private const string ServiceFolder = @"StreamingAssets\WiiConsoleService";
    private const string ServiceProgram = "WiiConsoleService.exe";
    private const string CalibrationFileName = "BalanceBoardCalibration.json";
    #endregion

    #region Felder
    private static BalanceBoardManager current;
    private BalanceBoard balanceBoard;
    private Process balanceBoardProcess;
    private string calibrationFilePath;
    private string exePath;

    private List<float> coGXMax;
    private List<float> coGXMin;
    private List<float> coGYMax;
    private List<float> coGYMin;
    private List<float> weightKgMin;
    private List<float> weightLbMin;
    private List<float> topLeftKgMin;
    private List<float> topRightKgMin;
    private List<float> bottomLeftKgMin;
    private List<float> bottomRightKgMin;
    private List<float> topLeftLbMin;
    private List<float> topRightLbMin;
    private List<float> bottomLeftLbMin;
    private List<float> bottomRightLbMin;

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

    public BalanceBoardState BalanceBoardState
    {
        get;
        private set;
    }
    #endregion

    #region Konstruktor
    private BalanceBoardManager()
    {
        this.exePath = Path.Combine(Application.dataPath, ServiceFolder);
        this.exePath = Path.Combine(exePath, ServiceProgram);
        Log(exePath);
        this.calibrationFilePath = Path.Combine(ConfigManager.Current.GameDirectoryPath, CalibrationFileName);

        this.BalanceBoardState = new BalanceBoardState();

        coGXMax = new List<float>();
        coGXMin = new List<float>();
        coGYMax = new List<float>();
        coGYMin = new List<float>();
        weightKgMin = new List<float>();
        weightLbMin = new List<float>();
        topLeftKgMin = new List<float>();
        topRightKgMin = new List<float>();
        bottomLeftKgMin = new List<float>();
        bottomRightKgMin = new List<float>();
        topLeftLbMin = new List<float>();
        topRightLbMin = new List<float>();
        bottomLeftLbMin = new List<float>();
        bottomRightLbMin = new List<float>();
    }
    #endregion

    #region Implementierungen
    public void Connect()
    {
        if (Search())
        {
            Thread t = new Thread(new ParameterizedThreadStart(CreateAndConnect));
            t.Start(this.exePath);
        }
        else
        {
            // signal program to cancel connecting
            if (BalanceBoardConnectionChanged != null)
            {
                BalanceBoardConnectionChanged(this, new BalanceBoardConnectionChangedEventArgs(this.BalanceBoard));
            }
        }
    }

    private bool Search()
    {
        try
        {
            this.BalanceBoard = WiiInputManager.Current.FindWiiController(ControllerType.WiiBalanceBoard) as BalanceBoard;
            if (this.BalanceBoard != null)
            {
                return true;
            }
            return false;
        }
        catch (WiiControllerNotFoundException)
        {
            return false;
        }
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
            // Funktioniert im Release Build von Unity nicht
            //balanceBoardProcess.EnableRaisingEvents = true;
            //balanceBoardProcess.Exited += BalanceBoardProcess_Exited;
            balanceBoardProcess.Start();
            // Alternative zu Exited-Event
            Log("Service Process started. Wait for exit ...");
            balanceBoardProcess.WaitForExit();
            Log("Service Process exited");
            BalanceBoardProcess_Exited(balanceBoardProcess, null);
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
        if (process != null && process.HasExited && process.ExitCode == 0)
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

    public void GetUpdate()
    {
        if (this.IsBalanceBoardConnected)
        {
            BalanceBoard.GetUpdate();

            this.BalanceBoardState.WeightKg = Math.Abs(this.BalanceBoardState.WeightKgMin
                - this.BalanceBoard.WiiControllerState.BalanceBoardState.WeightKg);
            this.BalanceBoardState.WeightLb = Math.Abs(this.BalanceBoardState.WeightLbMin
                - this.BalanceBoard.WiiControllerState.BalanceBoardState.WeightLb);

            this.BalanceBoardState.SensorValuesKg.TopLeft = Math.Abs(this.BalanceBoardState.SensorValuesKgMin.TopLeft
                - this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.TopLeft);
            this.BalanceBoardState.SensorValuesKg.TopRight = Math.Abs(this.BalanceBoardState.SensorValuesKgMin.TopRight
                - this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.TopRight);
            this.BalanceBoardState.SensorValuesKg.BottomLeft = Math.Abs(this.BalanceBoardState.SensorValuesKgMin.BottomLeft
                - this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomLeft);
            this.BalanceBoardState.SensorValuesKg.BottomRight = Math.Abs(this.BalanceBoardState.SensorValuesKgMin.BottomRight
                - this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomRight);

            this.BalanceBoardState.SensorValuesLb.TopLeft = Math.Abs(this.BalanceBoardState.SensorValuesLbMin.TopLeft
                - this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.TopLeft);
            this.BalanceBoardState.SensorValuesLb.TopRight = Math.Abs(this.BalanceBoardState.SensorValuesLbMin.TopRight
                - this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.TopRight);
            this.BalanceBoardState.SensorValuesLb.BottomLeft = Math.Abs(this.BalanceBoardState.SensorValuesLbMin.BottomLeft
                - this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.BottomLeft);
            this.BalanceBoardState.SensorValuesLb.BottomRight = Math.Abs(this.BalanceBoardState.SensorValuesLbMin.BottomRight
                - this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.BottomRight);

            this.BalanceBoardState.CenterOfGravity.X = this.BalanceBoard.WiiControllerState.BalanceBoardState.CenterOfGravity.X;
            this.BalanceBoardState.CenterOfGravity.Y = this.BalanceBoard.WiiControllerState.BalanceBoardState.CenterOfGravity.Y;
        }
    }

    /// <summary>
    /// Person steht auf dem Board
    /// </summary>
    public void CaptureMax()
    {
        if (this.IsBalanceBoardConnected)
        {
            coGXMax.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.CenterOfGravity.X);
            coGYMax.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.CenterOfGravity.Y);
        }
    }

    /// <summary>
    /// Person steht nicht auf dem Board
    /// </summary>
    public void CaptureMin()
    {
        if (this.IsBalanceBoardConnected)
        {
            coGXMin.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.CenterOfGravity.X);
            coGYMin.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.CenterOfGravity.Y);
            weightKgMin.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.WeightKg);
            weightLbMin.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.WeightLb);
            topLeftKgMin.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.TopLeft);
            topRightKgMin.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.TopRight);
            bottomLeftKgMin.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomLeft);
            bottomRightKgMin.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomRight);
            topLeftLbMin.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.TopLeft);
            topRightLbMin.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.TopRight);
            bottomLeftLbMin.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.BottomLeft);
            bottomRightLbMin.Add(this.BalanceBoard.WiiControllerState.BalanceBoardState.SensorValuesLb.BottomRight);
        }
    }

    public void CaptureFinished()
    {
        this.BalanceBoardState.CenterOfGravityMax.X = coGXMax.Average();
        this.BalanceBoardState.CenterOfGravityMin.X = coGXMin.Average();
        this.BalanceBoardState.CenterOfGravityMax.Y = coGYMax.Average();
        this.BalanceBoardState.CenterOfGravityMin.Y = coGYMin.Average();
        this.BalanceBoardState.WeightKgMin = weightKgMin.Average();
        this.BalanceBoardState.WeightLbMin = weightLbMin.Average();
        this.BalanceBoardState.SensorValuesKgMin.TopLeft = topLeftKgMin.Average();
        this.BalanceBoardState.SensorValuesKgMin.TopRight = topRightKgMin.Average();
        this.BalanceBoardState.SensorValuesKgMin.BottomLeft = bottomLeftKgMin.Average();
        this.BalanceBoardState.SensorValuesKgMin.BottomRight = bottomRightKgMin.Average();
        this.BalanceBoardState.SensorValuesLbMin.TopLeft = topLeftLbMin.Average();
        this.BalanceBoardState.SensorValuesLbMin.TopRight = topRightLbMin.Average();
        this.BalanceBoardState.SensorValuesLbMin.BottomLeft = bottomLeftLbMin.Average();
        this.BalanceBoardState.SensorValuesLbMin.BottomRight = bottomRightLbMin.Average();

        coGXMax.Clear();
        coGXMin.Clear();
        coGYMax.Clear();
        coGYMin.Clear();
        weightKgMin.Clear();
        weightLbMin.Clear();
        topLeftKgMin.Clear();
        topRightKgMin.Clear();
        bottomLeftKgMin.Clear();
        bottomRightKgMin.Clear();
        topLeftLbMin.Clear();
        topRightLbMin.Clear();
        bottomLeftLbMin.Clear();
        bottomRightLbMin.Clear();
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

public class BalanceBoardState
{
    public BalanceBoardSensorsF SensorValuesKg;

    public BalanceBoardSensorsF SensorValuesLb;

    public BalanceBoardSensorsF SensorValuesKgMin;

    public BalanceBoardSensorsF SensorValuesLbMin;

    public PointF CenterOfGravity;

    public PointF CenterOfGravityMin;

    public PointF CenterOfGravityMax;

    public float WeightKg;

    public float WeightLb;

    public float WeightKgMin;

    public float WeightLbMin;
}
