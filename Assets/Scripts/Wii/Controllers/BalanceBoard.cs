using System.Diagnostics;
using Wii.Contracts;

namespace Wii.Controllers
{
    public sealed class BalanceBoard : WiiController
    {
        #region Constants
        // length between board sensors
        private const int BSL = 43;

        // width between board sensors
        private const int BSW = 24;

        // kilograms to pounds
        private const float KG2LB = 2.20462262f;

        /// <summary>
        /// Wii Fit Balance Board controller
        /// </summary>
        private const long BalanceBoardType = 0x0000a4200402;
        #endregion

        #region Constructor
        internal BalanceBoard()
        {
        }
        #endregion

        #region Properties
        public override ControllerType ControllerType
        {
            get
            {
                return ControllerType.WiiBalanceBoard;
            }
        }
        #endregion

        #region Implements

        /// <summary>
        /// Set the LED on the BalanceBoard. Has only one LED to set.
        /// </summary>
        /// <param name="ledOn"></param>
        public void SetLED(bool ledOn)
        {
            this.SetLEDs(ledOn);
        }

        /// <summary>
        /// Set BalanceBoard reporting mode
        /// </summary>
        /// <param name="type">Report type</param>
        /// <param name="continuous">Continuous data</param>
        internal override void SetDataReportingMode(bool continuous)
        {
            // BalanceBoard has CoreButtonsWithExtension 0x34 ReportMode
            InputReport type = InputReport.CoreButtonsWithExtension;

            ClearReport();

            buff[0] = (byte)OutputReport.DataReportingMode;
            buff[1] = (byte)(continuous ? 0x04 : 0x00);
            buff[2] = (byte)type;

            // Write
            if (!WriteReport())
            {
                Debug.WriteLine("Write failed: DataReporting");
            }
            else
            {
                // Read
                if (!ReadReport())
                {
                    Debug.WriteLine("Read failed: DataReporting");
                }
            }
        }

        /// <summary>
        /// Read calibration information stored on WiiController
        /// </summary>
        internal override void SetCalibrationInfo()
        {
            // this appears to change the report type to 0x31 Core Buttons and Accelerometer 
            byte[] buff = ReadData(0x0016, 7);
        }

        /// <summary>
        /// Handles setting up an extension when plugged in
        /// </summary>
        internal override void Initialize()
        {
            WriteData(REGISTER_EXTENSION_INIT_1, 0x55);
            if (!ReadReport()) // AcknowledgeOutputReport
            {
                Debug.WriteLine("Read failed: init");
            }

            WriteData(REGISTER_EXTENSION_INIT_2, 0x00);
            if (!ReadReport()) // AcknowledgeOutputReport
            {
                Debug.WriteLine("Read failed: init");
            }

            byte[] buff = ReadData(REGISTER_EXTENSION_TYPE, 6);
            long type = ((long)buff[0] << 40) | ((long)buff[1] << 32) | ((long)buff[2]) << 24 | ((long)buff[3]) << 16 | ((long)buff[4]) << 8 | buff[5];

            Debug.WriteLine("BalanceBoradType: " + (type == BalanceBoardType));

            if (type == BalanceBoardType)
            {
                buff = ReadData(REGISTER_EXTENSION_CALIBRATION, 32);

                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg0.TopRight = (short)((short)buff[4] << 8 | buff[5]);
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg0.BottomRight = (short)((short)buff[6] << 8 | buff[7]);
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg0.TopLeft = (short)((short)buff[8] << 8 | buff[9]);
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg0.BottomLeft = (short)((short)buff[10] << 8 | buff[11]);

                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg17.TopRight = (short)((short)buff[12] << 8 | buff[13]);
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg17.BottomRight = (short)((short)buff[14] << 8 | buff[15]);
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg17.TopLeft = (short)((short)buff[16] << 8 | buff[17]);
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg17.BottomLeft = (short)((short)buff[18] << 8 | buff[19]);

                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg34.TopRight = (short)((short)buff[20] << 8 | buff[21]);
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg34.BottomRight = (short)((short)buff[22] << 8 | buff[23]);
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg34.TopLeft = (short)((short)buff[24] << 8 | buff[25]);
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg34.BottomLeft = (short)((short)buff[26] << 8 | buff[27]);

                // ReportMode as continous
                SetDataReportingMode(true);
            }
        }

        /// <summary>
        /// Parse data from an extension controller
        /// </summary>
        /// <param name="buff">Data buffer</param>
        /// <param name="offset">Offset into data buffer</param>
        internal override void ParseExtension(byte[] buff, int offset)
        {
            this.WiiControllerState.BalanceBoardState.SensorValuesRaw.TopRight = (short)((short)buff[offset + 0] << 8 | buff[offset + 1]);
            this.WiiControllerState.BalanceBoardState.SensorValuesRaw.BottomRight = (short)((short)buff[offset + 2] << 8 | buff[offset + 3]);
            this.WiiControllerState.BalanceBoardState.SensorValuesRaw.TopLeft = (short)((short)buff[offset + 4] << 8 | buff[offset + 5]);
            this.WiiControllerState.BalanceBoardState.SensorValuesRaw.BottomLeft = (short)((short)buff[offset + 6] << 8 | buff[offset + 7]);

            this.WiiControllerState.BalanceBoardState.SensorValuesKg.TopLeft = GetBalanceBoardSensorValue(
                this.WiiControllerState.BalanceBoardState.SensorValuesRaw.TopLeft,
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg0.TopLeft,
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg17.TopLeft,
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg34.TopLeft);

            this.WiiControllerState.BalanceBoardState.SensorValuesKg.TopRight = GetBalanceBoardSensorValue(
                this.WiiControllerState.BalanceBoardState.SensorValuesRaw.TopRight,
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg0.TopRight,
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg17.TopRight,
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg34.TopRight);

            this.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomLeft = GetBalanceBoardSensorValue(
                this.WiiControllerState.BalanceBoardState.SensorValuesRaw.BottomLeft,
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg0.BottomLeft,
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg17.BottomLeft,
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg34.BottomLeft);

            this.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomRight = GetBalanceBoardSensorValue(
                this.WiiControllerState.BalanceBoardState.SensorValuesRaw.BottomRight,
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg0.BottomRight,
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg17.BottomRight,
                this.WiiControllerState.BalanceBoardState.CalibrationInfo.Kg34.BottomRight);

            this.WiiControllerState.BalanceBoardState.SensorValuesLb.TopLeft =
                (this.WiiControllerState.BalanceBoardState.SensorValuesKg.TopLeft * KG2LB);

            this.WiiControllerState.BalanceBoardState.SensorValuesLb.TopRight =
                (this.WiiControllerState.BalanceBoardState.SensorValuesKg.TopRight * KG2LB);

            this.WiiControllerState.BalanceBoardState.SensorValuesLb.BottomLeft =
                (this.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomLeft * KG2LB);

            this.WiiControllerState.BalanceBoardState.SensorValuesLb.BottomRight =
                (this.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomRight * KG2LB);

            this.WiiControllerState.BalanceBoardState.WeightKg =
                (this.WiiControllerState.BalanceBoardState.SensorValuesKg.TopLeft +
                this.WiiControllerState.BalanceBoardState.SensorValuesKg.TopRight +
                this.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomLeft +
                this.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomRight) / 4.0f;

            this.WiiControllerState.BalanceBoardState.WeightLb =
                (this.WiiControllerState.BalanceBoardState.SensorValuesLb.TopLeft +
                this.WiiControllerState.BalanceBoardState.SensorValuesLb.TopRight +
                this.WiiControllerState.BalanceBoardState.SensorValuesLb.BottomLeft +
                this.WiiControllerState.BalanceBoardState.SensorValuesLb.BottomRight) / 4.0f;

            float Kx =
                (this.WiiControllerState.BalanceBoardState.SensorValuesKg.TopLeft +
                this.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomLeft) /
                (this.WiiControllerState.BalanceBoardState.SensorValuesKg.TopRight +
                this.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomRight);

            float Ky =
                (this.WiiControllerState.BalanceBoardState.SensorValuesKg.TopLeft +
                this.WiiControllerState.BalanceBoardState.SensorValuesKg.TopRight) /
                (this.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomLeft +
                this.WiiControllerState.BalanceBoardState.SensorValuesKg.BottomRight);

            this.WiiControllerState.BalanceBoardState.CenterOfGravity.X = ((float)(Kx - 1) / (float)(Kx + 1)) * (float)(-BSL / 2);
            this.WiiControllerState.BalanceBoardState.CenterOfGravity.Y = ((float)(Ky - 1) / (float)(Ky + 1)) * (float)(-BSW / 2);
        }

        private float GetBalanceBoardSensorValue(short sensor, short min, short mid, short max)
        {
            if (max == mid || mid == min)
            {
                return 0;
            }

            if (sensor < mid)
            {
                return 68.0f * ((float)(sensor - min) / (mid - min));
            }
            else
            {
                return 68.0f * ((float)(sensor - mid) / (max - mid)) + 68.0f;
            }
        }
        #endregion
    }
}
