namespace Wii.Input.Contracts
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
        /// Button and extension controller data
        /// </summary>
        CoreButtonsWithExtension = 0x34,
    }
}
