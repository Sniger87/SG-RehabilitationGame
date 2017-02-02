using System;

namespace Wii.Contracts
{
    /// <summary>
    /// The 4 sensors on the Balance Board (short values)
    /// </summary>
    [Serializable]
    [DataContract]
    public struct BalanceBoardSensors
    {
        /// <summary>
        /// Sensor at top right
        /// </summary>
        [DataMember]
        public short TopRight;
        /// <summary>
        /// Sensor at top left
        /// </summary>
        [DataMember]
        public short TopLeft;
        /// <summary>
        /// Sensor at bottom right
        /// </summary>
        [DataMember]
        public short BottomRight;
        /// <summary>
        /// Sensor at bottom left
        /// </summary>
        [DataMember]
        public short BottomLeft;
    }

    /// <summary>
    /// The 4 sensors on the Balance Board (float values)
    /// </summary>
    [Serializable]
    [DataContract]
    public struct BalanceBoardSensorsF
    {
        /// <summary>
        /// Sensor at top right
        /// </summary>
        [DataMember]
        public float TopRight;
        /// <summary>
        /// Sensor at top left
        /// </summary>
        [DataMember]
        public float TopLeft;
        /// <summary>
        /// Sensor at bottom right
        /// </summary>
        [DataMember]
        public float BottomRight;
        /// <summary>
        /// Sensor at bottom left
        /// </summary>
        [DataMember]
        public float BottomLeft;
    }
}
