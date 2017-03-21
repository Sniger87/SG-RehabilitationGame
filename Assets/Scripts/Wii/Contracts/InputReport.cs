namespace Wii.Contracts
{
    /// <summary>
    /// The report format in which the WiiController should return data
    /// </summary>
    public enum InputReport : byte
    {
        /// <summary>
        /// Status report
        /// </summary>
        Status = 0x20,
        /// <summary>
        /// Read data from memory location
        /// </summary>
        ReadMemoryData = 0x21,
        /// <summary>
        /// Register write complete
        /// </summary>
        AcknowledgeOutputReport = 0x22,
        /// <summary>
        /// Button data only
        /// </summary>
        CoreButtons = 0x30,
        /// <summary>
        /// Button and Accelerometer (3 Accelerometer bytes)
        /// </summary>
        CoreButtonsWith3Accel = 0x31,
        /// <summary>
        /// Button and extension controller data (8 Extension bytes)
        /// </summary>
        CoreButtonsWith8Extension = 0x32,
        /// <summary>
        /// Button and extension controller data (3 Accelerometer and 12 IR bytes)
        /// </summary>
        CoreButtonsWith3AccelAnd12IR = 0x33,
        /// <summary>
        /// Button and extension controller data (19 Extension bytes)
        /// </summary>
        CoreButtonsWith19Extension = 0x34,
        /// <summary>
        /// Button and extension controller data (3 Accelerometer and 16 Extension bytes)
        /// </summary>
        CoreButtonsWith3AccelAnd16Extension = 0x35,
        /// <summary>
        /// Button and extension controller data (10 IR and 9 Extension bytes)
        /// </summary>
        CoreButtonsWith10IRAnd9Extension = 0x36,
        /// <summary>
        /// Button and extension controller data (3 Accelerometer, 10 IR and 6 Extension bytes)
        /// </summary>
        CoreButtonsWith3AccelAnd10IRAnd6Extension = 0x37,
    }
}
