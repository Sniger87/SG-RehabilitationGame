using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using Wii.Events;
using Wii.Contracts;
using Wii.Exceptions;
using Wii.DesktopFacades;
using Wii.Collections;
using System.Linq;
using System.Collections.Generic;

//  See http://www.codeplex.com/WiiControllerLib
//  See http://wiibrew.org/wiki/WiiController

namespace Wii.Controllers
{
    /// <summary>
    /// Implementation of WiiController
    /// </summary>
    public abstract class WiiController : IDisposable
    {
        #region Constants
        // we could find this out the hard way using HID, it's 22
        protected const int REPORT_LENGTH = 22;

        // WiiController registers
        protected const int REGISTER_EXTENSION_INIT_1 = 0x04a400f0;
        protected const int REGISTER_EXTENSION_INIT_2 = 0x04a400fb;
        protected const int REGISTER_EXTENSION_TYPE = 0x04a400fa;
        protected const int REGISTER_EXTENSION_CALIBRATION = 0x04a40020; // size: 32
        protected const int REGISTER_EXTENSION_CALIBRATION_BALANCEBOARD = 0x04a40024; // size: 24
        #endregion

        #region Fields
        /// <summary>
        /// Event raised when WiiController state is changed
        /// </summary>
        public event EventHandler<WiiControllerStateChangedEventArgs> WiiControllerStateChanged;

        // read/write handle to the device
        private SafeFileHandle safeFileHandle;

        // current state of controller
        private WiiControllerState wiiControllerState;

        // HID device path of this WiiController
        private string devicePath = string.Empty;

        // report buffer
        internal readonly byte[] buff = new byte[REPORT_LENGTH];

        // read data buffer
        private byte[] readBuff;

        // address to read from
        private int address;

        // size of requested read
        private short size;

        // store if WiiController is connected
        private bool isConnected;
        private bool isInitialized;

        // event for read data processing
        private readonly ManualResetEvent mre = new ManualResetEvent(false);
        private readonly AutoResetEvent readDone = new AutoResetEvent(false);
        private readonly AutoResetEvent writeDone = new AutoResetEvent(false);

        // event for status report
        private readonly AutoResetEvent statusDone = new AutoResetEvent(false);

        private Thread readThread;
        private bool readAsync;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        internal WiiController()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Current WiiController state
        /// </summary>
        public WiiControllerState WiiControllerState
        {
            get { return this.wiiControllerState; }
            internal set
            {
                if (value != this.wiiControllerState)
                {
                    this.wiiControllerState = value;
                }
            }
        }

        /// <summary>
        /// Current SafeFileHandle
        /// </summary>
        internal SafeFileHandle SafeFileHandle
        {
            get { return this.safeFileHandle; }
            set
            {
                if (value != this.safeFileHandle)
                {
                    this.safeFileHandle = value;
                }
            }
        }

        /// <summary>
        /// HID device path for this WiiController (valid until WiiController is disconnected)
        /// </summary>
        internal string HIDDevicePath
        {
            get { return this.devicePath; }
            set
            {
                if (value != this.devicePath)
                {
                    this.devicePath = value;
                }
            }
        }

        /// <summary>
        /// Gives the type of WiiController
        /// </summary>
        public abstract ControllerType ControllerType
        {
            get;
        }

        /// <summary>
        /// Returns if WiiController is connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
            private set
            {
                if (value != this.isConnected)
                {
                    this.isConnected = value;
                }
            }
        }

        /// <summary>
        /// Returns if WiiController is initialized
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                return this.isInitialized;
            }
            protected set
            {
                if (value != this.isInitialized)
                {
                    this.isInitialized = value;
                }
            }
        }
        #endregion

        #region Implements
        /// <summary>
        /// Set WiiController reporting mode
        /// </summary>
        /// <param name="type">Report type</param>
        /// <param name="continuous">Continuous data</param>
        internal abstract void SetDataReportingMode(bool continuous);

        internal abstract void Initialize();

        /// <summary>
        /// Read calibration information stored on WiiController
        /// </summary>
        internal abstract void SetCalibrationInfo();

        /// <summary>
        /// Initialize with information and store it on WiiController
        /// </summary>
        internal abstract void Initialize(ICalibrationInfo calibrationInfo);

        /// <summary>
        /// Parse data from an extension controller
        /// </summary>
        /// <param name="buff">Data buffer</param>
        /// <param name="offset">Offset into data buffer</param>
        internal abstract void ParseExtension(byte[] buff, int offset);

        /// <summary>
        /// Set the LEDs on the BalanceBoard
        /// </summary>
        /// <param name="led1">LED 1</param>
        /// <param name="led2">LED 2</param>
        /// <param name="led3">LED 3</param>
        /// <param name="led4">LED 4</param>
        public virtual void SetLEDs(bool led1 = false, bool led2 = false, bool led3 = false, bool led4 = false)
        {
            if (!this.IsConnected)
            {
                return;
            }

            // BalanceBoard has only one LED, ignore other input arguments
            this.WiiControllerState.LEDState.LED1 = led1;
            this.WiiControllerState.LEDState.LED2 = led2;
            this.WiiControllerState.LEDState.LED3 = led3;
            this.WiiControllerState.LEDState.LED4 = led4;

            // Create PlayerLED ReportType
            ClearReport();

            buff[0] = (byte)OutputReport.PlayerLEDs;
            buff[1] = (byte)(
                        (led1 ? 0x10 : 0x00) |  // 0001 0000 -> 16
                        (led2 ? 0x20 : 0x00) |  // 0010 0000 -> 32
                        (led3 ? 0x40 : 0x00) |  // 0100 0000 -> 64
                        (led4 ? 0x80 : 0x00));  // 1000 0000 -> 128
                                                // All on: 1111 0000 -> 240

            // Write
            if (!WriteReport())
            {
                Log("Write failed: set led");
            }
        }

        /// <summary>
        /// Retrieve the current status of the WiiController.
        /// </summary>
        public void GetStatus()
        {
            if (!this.IsConnected)
            {
                return;
            }

            ClearReport();

            buff[0] = (byte)OutputReport.Status;
            buff[1] = (byte)0x00;

            // Write
            if (!WriteReport())
            {
                Log("Write failed: get status");
            }

            // signal the status report finished
            if (!statusDone.WaitOne(3000, false))
            {
                Log("Timed out waiting for status report");
            }
        }

        /// <summary>
        /// Parse a report sent by the WiiController
        /// </summary>
        /// <param name="buff">Data buffer to parse</param>
        /// <returns>Returns a boolean noting whether an event needs to be posted</returns>
        private bool ParseInputReport(byte[] buff)
        {
            InputReport type = (InputReport)buff[0];
            //Log(type.ToString());
            switch (type)
            {
                case InputReport.CoreButtons:
                    ParseButtons(buff);
                    break;
                case InputReport.CoreButtonsWithExtension:
                    ParseButtons(buff);
                    ParseExtension(buff, 3);
                    break;
                case InputReport.Status:
                    ParseButtons(buff);
                    this.WiiControllerState.BatteryRaw = buff[6];
                    this.WiiControllerState.Battery = (((100.0f * 48.0f * (float)((int)buff[6] / 48.0f))) / 192.0f);

                    // get the real LED values in case the values from SetLEDs() somehow becomes out of sync, which really shouldn't be possible
                    this.WiiControllerState.LEDState.LED1 = (buff[3] & 0x10) != 0;
                    this.WiiControllerState.LEDState.LED2 = (buff[3] & 0x20) != 0;
                    this.WiiControllerState.LEDState.LED3 = (buff[3] & 0x40) != 0;
                    this.WiiControllerState.LEDState.LED4 = (buff[3] & 0x80) != 0;

                    // extension connected?
                    // BalanceBoard is a extension too
                    bool extension = (buff[3] & 0x02) != 0;
                    Log("Extension: " + extension.ToString());

                    if (extension)
                    {
                        Thread initializeThread = BeginAsyncRead();
                        Thread.Sleep(100);

                        Initialize();
                        if (initializeThread != null)
                        {
                            initializeThread.Abort();
                            initializeThread = null;
                        }
                    }

                    statusDone.Set();
                    break;
                case InputReport.ReadMemoryData:
                    ParseButtons(buff);
                    ParseReadData(buff);
                    break;
                case InputReport.AcknowledgeOutputReport:
                    Log("ack: " + buff[0].ToString() + " " + buff[1].ToString() + " " + buff[2].ToString() + " " + buff[3].ToString() + " " + buff[4].ToString());
                    writeDone.Set();
                    break;
                default:
                    Log("Unknown report type: " + type.ToString("x"));
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Decrypts data sent from the extension to the WiiController
        /// </summary>
        /// <param name="buff">Data buffer</param>
        /// <returns>Byte array containing decoded data</returns>
        private byte[] DecryptBuffer(byte[] buff)
        {
            for (int i = 0; i < buff.Length; i++)
            {
                buff[i] = (byte)(((buff[i] ^ 0x17) + 0x17) & 0xff);
            }

            return buff;
        }

        /// <summary>
        /// Parses a standard button report into the ButtonState struct
        /// </summary>
        /// <param name="buff">Data buffer</param>
        private void ParseButtons(byte[] buff)
        {
            // Power-Button wird beim BalanceBoard auf A-Button gemapt
            this.WiiControllerState.ButtonState.A = (buff[2] & 0x08) != 0;
        }

        /// <summary>
        /// Parse data returned from a read report
        /// </summary>
        /// <param name="buff">Data buffer</param>
        private void ParseReadData(byte[] buff)
        {
            if ((buff[3] & 0x08) != 0)
            {
                throw new WiiControllerException("Error reading data from WiiController: Bytes do not exist.");
            }

            if ((buff[3] & 0x07) != 0)
            {
                throw new WiiControllerException("Error reading data from WiiController: Attempt to read from write-only registers.");
            }

            // get our size and offset from the report
            int size = (buff[3] >> 4) + 1;
            int offset = (buff[4] << 8 | buff[5]);

            // add it to the buffer
            Array.Copy(buff, 6, readBuff, offset - this.address, size);

            // if we've read it all, set the event
            if (this.address + this.size == offset + size)
            {
                readDone.Set();
            }
        }

        /// <summary>
        /// Initialize the report data buffer
        /// </summary>
        internal void ClearReport()
        {
            Array.Clear(buff, 0, REPORT_LENGTH);
        }

        /// <summary>
        /// Write a report to the WiiController
        /// </summary>
        internal bool WriteReport()
        {
            Log("WriteReport: " + buff[0].ToString("x"));

            bool result = HIDImports.HidD_SetOutputReport(this.SafeFileHandle.DangerousGetHandle(), buff, (uint)buff.Length);

            Log("WriteReport: " + result.ToString());

            if (this.buff[0] == (byte)OutputReport.WriteMemory)
            {
                Log("Wait");
                if (!writeDone.WaitOne(1000, false))
                {
                    Log("Wait failed");
                }
            }

            return result;
        }

        /// <summary>
        /// Read data or register from WiiController
        /// </summary>
        /// <param name="address">Address to read</param>
        /// <param name="size">Length to read</param>
        /// <returns>Data buffer</returns>
        internal byte[] ReadData(int address, short size)
        {
            ClearReport();

            readBuff = new byte[size];
            this.address = address & 0xffff;
            this.size = size;

            buff[0] = (byte)OutputReport.ReadMemory;
            buff[1] = (byte)(((address & 0xff000000) >> 24) | 0x00);
            buff[2] = (byte)((address & 0x00ff0000) >> 16);
            buff[3] = (byte)((address & 0x0000ff00) >> 8);
            buff[4] = (byte)(address & 0x000000ff);

            buff[5] = (byte)((size & 0xff00) >> 8);
            buff[6] = (byte)(size & 0xff);

            // Write
            if (!WriteReport())
            {
                Log("Write failed: ReadData");
            }

            if (!readDone.WaitOne(1000, false))
            {
                Log("Error reading data from Wiimote, is it connected?");
            }

            return readBuff;
        }

        /// <summary>
        /// Write a single byte to the WiiController
        /// </summary>
        /// <param name="address">Address to write</param>
        /// <param name="data">Byte to write</param>
        internal void WriteData(int address, byte data)
        {
            WriteData(address, 1, new byte[] { data });
        }

        /// <summary>
        /// Write a byte array to a specified address
        /// </summary>
        /// <param name="address">Address to write</param>
        /// <param name="size">Length of buffer</param>
        /// <param name="buff">Data buffer</param>

        internal void WriteData(int address, byte size, byte[] buff)
        {
            ClearReport();

            this.buff[0] = (byte)OutputReport.WriteMemory;
            this.buff[1] = (byte)(((address & 0xff000000) >> 24) | 0x00);
            this.buff[2] = (byte)((address & 0x00ff0000) >> 16);
            this.buff[3] = (byte)((address & 0x0000ff00) >> 8);
            this.buff[4] = (byte)(address & 0x000000ff);
            this.buff[5] = size;
            Array.Copy(buff, 0, this.buff, 6, size);

            if (!WriteReport())
            {
                Log("Write failed: WriteData");
            }
        }

        public void Disconnect()
        {
            if (this.IsConnected)
            {
                SetLEDs();
                Dispose();
            }
        }

        public void Connect()
        {
            TryOpenAndConnect();
        }

        public void Connect(ICalibrationInfo calibrationInfo)
        {
            TryOpenAndConnect(onlyOpen: true);
            Initialize(calibrationInfo);
        }

        private void TryOpenAndConnect(bool onlyOpen = false)
        {
            if (this.IsConnected)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.HIDDevicePath))
            {
                this.HIDDevicePath = WiiInputManager.Current.SearchWiiController(this.ControllerType);
            }

            OpenWiiControllerDeviceHandle(this.HIDDevicePath, onlyOpen);
        }

        internal void OpenWiiControllerDeviceHandle(string devicePath, bool onlyOpen = false)
        {
            // open a read/write handle to our device using the DevicePath returned
            SafeFileHandle safeFileHandle = HIDImports.CreateFile(devicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);

            // create an attributes struct and initialize the size
            HIDImports.HIDD_ATTRIBUTES attrib = new HIDImports.HIDD_ATTRIBUTES();
            attrib.Size = Marshal.SizeOf(attrib);

            // get the attributes of the current device
            if (HIDImports.HidD_GetAttributes(safeFileHandle.DangerousGetHandle(), ref attrib))
            {
                // if the vendor and product IDs match up
                if (attrib.VendorID == WiiInputManager.VID)
                {
                    if (attrib.ProductID == WiiInputManager.PID_old || attrib.ProductID == WiiInputManager.PID_new)
                    {
                        // initialize
                        this.WiiControllerState = new WiiControllerState();
                        this.SafeFileHandle = safeFileHandle;
                        this.HIDDevicePath = devicePath;

                        if (onlyOpen)
                        {
                            // get calibration infos
                            this.SetCalibrationInfo();

                            this.IsConnected = true;

                            // set LED
                            this.SetLEDs(true, true, true, true);
                        }
                        else
                        {
                            // start reading
                            this.readThread = BeginAsyncRead(10);
                            Thread.Sleep(100);

                            // get calibration infos
                            this.SetCalibrationInfo();

                            this.IsConnected = true;

                            // get status from the WiiContoller
                            this.GetStatus();

                            // set LED
                            this.SetLEDs(true, true, true, true);

                            if (this.readThread != null)
                            {
                                this.readThread.Abort();
                                this.readThread = null;
                            }

                            this.readAsync = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Start reading asynchronously from the controller
        /// </summary>
        protected Thread BeginAsyncRead(int threadSleepTime = 0)
        {
            this.readAsync = true;

            Log("Read Thread starting");

            // Start new Thread for reading
            Thread t = new Thread(OnReadData);
            Log("Thread: " + (t != null).ToString());
            t.Start(threadSleepTime);
            return t;
        }

        /// <summary>
        /// Reading in another Thread
        /// </summary>
        private void OnReadData(object threadSleepTime)
        {
            Log("Read Thread started");

            try
            {
                int sleepTime = (int)threadSleepTime;
                while (readAsync)
                {
                    byte[] buff = new byte[REPORT_LENGTH];

                    uint numberOfBytesRead = 0;
                    NativeOverlapped overlapped = new NativeOverlapped();
                    overlapped.EventHandle = mre.SafeWaitHandle.DangerousGetHandle();
                    overlapped.OffsetHigh = 0;
                    overlapped.OffsetLow = 0;

                    bool result = HIDImports.ReadFile(this.SafeFileHandle.DangerousGetHandle(), buff, (uint)buff.Length, out numberOfBytesRead, ref overlapped);

                    Log("ReadReport: " + result.ToString());

                    if (!result)
                    {
                        uint lastError = HIDImports.GetLastError();
                        if (lastError != HIDImports.ERROR_IO_PENDING)
                        {
                            Log("Read failed: " + lastError.ToString("X"));
                            continue;
                        }
                        else
                        {
                            if (!HIDImports.GetOverlappedResult(this.SafeFileHandle.DangerousGetHandle(), ref overlapped, out numberOfBytesRead, true))
                            {
                                lastError = HIDImports.GetLastError();
                                Log("Read failed: " + lastError.ToString("X"));
                                continue;
                            }

                            if (overlapped.InternalHigh.ToInt32() == HIDImports.STATUS_PENDING || overlapped.InternalLow.ToInt32() == HIDImports.STATUS_PENDING)
                            {
                                Log("Read interrupted " + lastError.ToString("X"));
                                if (!HIDImports.CancelIo(this.SafeFileHandle.DangerousGetHandle()))
                                {
                                    lastError = HIDImports.GetLastError();
                                    Log("CancelIO failed: " + lastError.ToString("X"));
                                    continue;
                                }
                            }
                        }
                    }

                    // parse it
                    if (buff != null)
                    {
                        if (ParseInputReport(buff))
                        {
                            if (WiiControllerStateChanged != null)
                            {
                                WiiControllerStateChanged(this, new WiiControllerStateChangedEventArgs(this.WiiControllerState));
                            }
                        }
                    }

                    Thread.Sleep(sleepTime);
                }
            }
            catch (ThreadAbortException ex)
            {
                Log("Thread abort!: " + ex.Message);
            }
        }

        /// <summary>
        /// Reading
        /// </summary>
        private void ReadReport()
        {
            byte[] buff = new byte[REPORT_LENGTH];

            uint numberOfBytesRead = 0;
            NativeOverlapped overlapped = new NativeOverlapped();
            overlapped.EventHandle = mre.SafeWaitHandle.DangerousGetHandle();
            overlapped.OffsetHigh = 0;
            overlapped.OffsetLow = 0;

            bool result = HIDImports.ReadFile(this.SafeFileHandle.DangerousGetHandle(), buff, (uint)buff.Length, out numberOfBytesRead, ref overlapped);
            //Log("ReadReport: " + result.ToString());

            // parse it
            if (buff != null && result)
            {
                if (ParseInputReport(buff))
                {
                    if (WiiControllerStateChanged != null)
                    {
                        WiiControllerStateChanged(this, new WiiControllerStateChangedEventArgs(this.WiiControllerState));
                    }
                }
            }
        }

        public void GetUpdate()
        {
            ReadReport();
        }

        protected void Log(string content)
        {
            string path = Configurations.ConfigManager.Current.GameLogPath;
            FileIO.FileManager.AppendLog(path, content);
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose WiiController
        /// </summary>
        public void Dispose()
        {
            this.readAsync = false;
            this.IsConnected = false;
            this.IsInitialized = false;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose WiiController
        /// </summary>
        /// <param name="disposing">Disposing?</param>
        protected virtual void Dispose(bool disposing)
        {
            // close up our handles
            if (disposing)
            {
                // Close open Streams
                if (this.SafeFileHandle != null)
                {
                    this.SafeFileHandle.Close();
                    this.SafeFileHandle.Dispose();
                    this.SafeFileHandle = null;
                }
            }
        }
        #endregion
    }
}