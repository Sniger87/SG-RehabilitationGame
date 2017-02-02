using System;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Text;

namespace Wii.Contracts
{
    /// <summary>
    /// Win32 import information for use with the Wii library
    /// </summary>
    internal class HIDImports
    {
        //
        // Flags controlling what is included in the device information set built
        // by SetupDiGetClassDevs
        //
        public const int DIGCF_DEFAULT = 0x00000001; // only valid with DIGCF_DEVICEINTERFACE
        public const int DIGCF_PRESENT = 0x00000002;
        public const int DIGCF_ALLCLASSES = 0x00000004;
        public const int DIGCF_PROFILE = 0x00000008;
        public const int DIGCF_DEVICEINTERFACE = 0x00000010;

        /// <summary>
        /// Overlapped I/O operation is in progress.
        /// </summary>
        public const int ERROR_IO_PENDING = 997;
        /// <summary>
        /// The operation that was requested is pending completion.
        /// </summary>
        public const int STATUS_PENDING = 0x00000103;

        public static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [Flags]
        public enum EFileAttributes : uint
        {
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            Write_Through = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr RESERVED;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HIDD_ATTRIBUTES
        {
            public int Size;
            public short VendorID;
            public short ProductID;
            public short VersionNumber;
        }

        [DllImport(@"hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void HidD_GetHidGuid(out Guid gHid);

        [DllImport("hid.dll")]
        public static extern Boolean HidD_GetAttributes(IntPtr HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);

        [DllImport("hid.dll")]
        public extern static bool HidD_SetOutputReport(
            IntPtr HidDeviceObject,
            byte[] lpReportBuffer,
            uint ReportBufferLength);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern bool HidD_GetProductString(
            IntPtr HidDeviceObject,
            byte[] Buffer,
            Int32 BufferLength);

        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(
            ref Guid ClassGuid,
            [MarshalAs(UnmanagedType.LPTStr)] string Enumerator,
            IntPtr hwndParent,
            UInt32 Flags
            );

        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(
            IntPtr hDevInfo,
            IntPtr devInvo,
            ref Guid interfaceClassGuid,
            Int32 memberIndex,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
        );

        [DllImport(@"setupapi.dll", SetLastError = true)]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(
            IntPtr hDevInfo,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            IntPtr deviceInterfaceDetailData,
            UInt32 deviceInterfaceDetailDataSize,
            out UInt32 requiredSize,
            IntPtr deviceInfoData
        );

        [DllImport(@"setupapi.dll", SetLastError = true)]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(
            IntPtr hDevInfo,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
            UInt32 deviceInterfaceDetailDataSize,
            out UInt32 requiredSize,
            IntPtr deviceInfoData
        );

        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern UInt16 SetupDiDestroyDeviceInfoList(IntPtr hDevInfo);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] EFileAttributes flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(
            IntPtr hFile,
            [Out] byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            [In] ref System.Threading.NativeOverlapped lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
        // GetLastError is save!

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateEvent(
            IntPtr lpEventAttributes,
            bool bManualReset,
            bool bInitialState,
            string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetOverlappedResult(
            IntPtr hFile,
            [In] ref System.Threading.NativeOverlapped lpOverlapped,
            out uint lpNumberOfBytesTransferred,
            bool bWait);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CancelIo(IntPtr hFile);
    }
}
