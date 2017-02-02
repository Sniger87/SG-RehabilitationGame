namespace Wii.Contracts
{
    // WiiController output commands
    internal enum OutputReport : byte
    {
        Unknown = 0x10,
        PlayerLEDs = 0x11,
        DataReportingMode = 0x12,
        Status = 0x15,
        WriteMemory = 0x16,
        ReadMemory = 0x17,
    }
}
