using System;

namespace Wii.Input.Contracts
{
    /// <summary>
    /// Current state of LEDs
    /// </summary>
    [Serializable]
    [DataContract]
    public struct LEDState
    {
        /// <summary>
        /// LED on the WiiController
        /// </summary>
        [DataMember]
        public bool LED1, LED2, LED3, LED4;
        // Balance Board hat nur LED1
    }

    /// <summary>
    /// Current state of the Wii Fit Balance Board controller
    /// </summary>
    [Serializable]
    [DataContract]
    public struct BalanceBoardState
    {
        /// <summary>
        /// Calibration information for the Balance Board
        /// </summary>
        [DataMember]
        public BalanceBoardCalibrationInfo CalibrationInfo;
        /// <summary>
        /// Raw values of each sensor
        /// </summary>
        [DataMember]
        public BalanceBoardSensors SensorValuesRaw;
        /// <summary>
        /// Kilograms per sensor
        /// </summary>
        [DataMember]
        public BalanceBoardSensorsF SensorValuesKg;
        /// <summary>
        /// Pounds per sensor
        /// </summary>
        [DataMember]
        public BalanceBoardSensorsF SensorValuesLb;
        /// <summary>
        /// Total kilograms on the Balance Board
        /// </summary>
        [DataMember]
        public float WeightKg;
        /// <summary>
        /// Total pounds on the Balance Board
        /// </summary>
        [DataMember]
        public float WeightLb;
        /// <summary>
        /// Center of gravity of Balance Board user
        /// </summary>
        [DataMember]
        public PointF CenterOfGravity;
    }

    /// <summary>
    /// Current button state
    /// </summary>
    [Serializable]
    [DataContract]
    public struct ButtonState
    {
        /// <summary>
        /// Digital button on the WiiController
        /// </summary>
        [DataMember]
        //public bool A, B, Plus, Home, Minus, One, Two, Up, Down, Left, Right;
        public bool A; // Power Button maps to A Button
    }
}
