using System;

namespace Wii.Contracts
{
    /// <summary>
    /// Calibration information
    /// </summary>
    [Serializable]
    [DataContract]
    public struct BalanceBoardCalibrationInfo
    {
        /// <summary>
        /// Calibration information at 0kg
        /// </summary>
        [DataMember]
        public BalanceBoardSensors Kg0;
        /// <summary>
        /// Calibration information at 17kg
        /// </summary>
        [DataMember]
        public BalanceBoardSensors Kg17;
        /// <summary>
        /// Calibration information at 34kg
        /// </summary>
        [DataMember]
        public BalanceBoardSensors Kg34;
    }
}
