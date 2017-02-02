using System;

namespace Wii.Contracts
{
    /// <summary>
    /// Point structure for floating point 2D positions (X, Y)
    /// </summary>
    [Serializable]
    [DataContract]
    public struct PointF
    {
        /// <summary>
        /// X, Y coordinates of this point
        /// </summary>
        [DataMember]
        public float X, Y;

        /// <summary>
        /// Convert to human-readable string
        /// </summary>
        /// <returns>A string that represents the point</returns>
        public override string ToString()
        {
            return string.Format("{{X={0}, Y={1}}}", X, Y);
        }

    }

    /// <summary>
    /// Point structure for int 2D positions (X, Y)
    /// </summary>
    [Serializable]
    [DataContract]
    public struct Point
    {
        /// <summary>
        /// X, Y coordinates of this point
        /// </summary>
        [DataMember]
        public int X, Y;

        /// <summary>
        /// Convert to human-readable string
        /// </summary>
        /// <returns>A string that represents the point.</returns>
        public override string ToString()
        {
            return string.Format("{{X={0}, Y={1}}}", X, Y);
        }
    }
}
