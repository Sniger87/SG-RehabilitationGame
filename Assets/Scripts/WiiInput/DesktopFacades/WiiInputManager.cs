using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Wii.Input.Collections;
using Wii.Input.Contracts;
using Wii.Input.Controllers;
using Wii.Input.Exceptions;

namespace Wii.Input.DesktopFacades
{
    public class WiiInputManager
    {
        #region Constants
        // VID = Nintendo, PID = WiiController
        internal const int VID = 0x057e;
        // Wii Remote/old Wii Remote Plus
        internal const int PID_old = 0x0306;
        // new Wii Remote Plus
        internal const int PID_new = 0x0330;
        // new Name of WBB
        private const string WBBName_new = "Nintendo RVL-WBC-01";
        // old Name of WBB
        private const string WBBName_old = "Nintendo RVL-CNT-01";
        #endregion

        #region Fields
        private static WiiInputManager current;

        // delegate used for enumerating found WiiControllers
        private delegate void WiiControllerFoundDelegate(string devicePath, ControllerType controllerType);

        private WiiControllerCollection collection = new WiiControllerCollection();
        #endregion

        #region Constructor
        private WiiInputManager()
        {
        }
        #endregion

        #region Properties
        public static WiiInputManager Current
        {
            get
            {
                if (current == null)
                {
                    current = new WiiInputManager();
                }
                return current;
            }
        }
        #endregion

        #region Implements

        /// <summary>
        /// Finds WiiController with the Type connected to the system and return them
        /// </summary>
        public WiiController FindWiiController(ControllerType controllerType)
        {
            SearchWiiController(WiiControllerFound, controllerType, true);
            if (collection.Any())
            {
                return collection.First();
            }
            else
            {
                throw new WiiControllerNotFoundException();
            }
        }

        /// <summary>
        /// Finds all WiiControllers with the Type connected to the system and adds them to the collection
        /// </summary>
        public WiiControllerCollection FindWiiControllers(ControllerType controllerType)
        {
            SearchWiiController(WiiControllerFound, controllerType);
            if (collection.Any())
            {
                return collection;
            }
            else
            {
                throw new WiiControllerNotFoundException();
            }
        }

        private static void SearchWiiController(WiiControllerFoundDelegate wiiControllerFound, ControllerType controllerType, bool breakIfFoundOne = false)
        {
            int index = 0;
            bool found = false;
            Guid guid;
            SafeFileHandle handle;

            // get the GUID of the HID class
            HIDImports.HidD_GetHidGuid(out guid);

            // get a handle to all devices that are part of the HID class
            // Fun fact:  DIGCF_PRESENT worked on my machine just fine.  I reinstalled Vista, and now it no longer finds the WiiController with that parameter enabled...
            IntPtr hDevInfo = HIDImports.SetupDiGetClassDevs(ref guid, null, IntPtr.Zero, HIDImports.DIGCF_DEVICEINTERFACE);// | HIDImports.DIGCF_PRESENT);

            // create a new interface data struct and initialize its size
            HIDImports.SP_DEVICE_INTERFACE_DATA diData = new HIDImports.SP_DEVICE_INTERFACE_DATA();
            diData.cbSize = Marshal.SizeOf(diData);

            // get a device interface to a single device (enumerate all devices)
            while (HIDImports.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref guid, index, ref diData))
            {
                UInt32 size;

                // get the buffer size for this device detail instance (returned in the size parameter)
                HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, IntPtr.Zero, 0, out size, IntPtr.Zero);

                // create a detail struct and set its size
                HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA diDetail = new HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA();

                // on Win x86, cbSize must be 5 for some reason.  On x64, apparently 8 is what it wants.
                diDetail.cbSize = (uint)(IntPtr.Size == 8 ? 8 : 5);

                // actually get the detail struct
                if (HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, ref diDetail, size, out size, IntPtr.Zero))
                {
                    Debug.WriteLine(string.Format("{0}: {1} - {2}", index, diDetail.DevicePath, Marshal.GetLastWin32Error()));

                    // open a read/write handle to our device using the DevicePath returned
                    bool closeHandle = true;
                    handle = HIDImports.CreateFile(diDetail.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);

                    // create an attributes struct and initialize the size
                    HIDImports.HIDD_ATTRIBUTES attrib = new HIDImports.HIDD_ATTRIBUTES();
                    attrib.Size = Marshal.SizeOf(attrib);

                    // get the attributes of the current device
                    if (HIDImports.HidD_GetAttributes(handle.DangerousGetHandle(), ref attrib))
                    {
                        // if the vendor and product IDs match up
                        if (attrib.VendorID == WiiInputManager.VID)
                        {
                            if (attrib.ProductID == WiiInputManager.PID_old || attrib.ProductID == WiiInputManager.PID_new)
                            {
                                found = CheckWiiControllerType(handle, controllerType, diDetail, wiiControllerFound);

                                if (found)
                                {
                                    // it's the right WiiController
                                    Debug.WriteLine("Found one and open!");
                                }

                                // stop at first found
                                if (breakIfFoundOne)
                                {
                                    break;
                                }
                                // dont close the handle
                                closeHandle = false;
                            }
                        }
                    }
                    if (closeHandle)
                    {
                        handle.Close();
                    }
                }
                else
                {
                    // failed to get the detail struct
                    throw new WiiControllerException("SetupDiGetDeviceInterfaceDetail failed on index " + index);
                }

                // move to the next device
                index++;
            }

            // clean up our list
            HIDImports.SetupDiDestroyDeviceInfoList(hDevInfo);

            // if we didn't find a WiiController, throw an exception
            if (!found)
            {
                throw new WiiControllerNotFoundException("No WiiControllers found in HID device list.");
            }
        }

        private static bool CheckWiiControllerType(SafeFileHandle handle, ControllerType controllerType, HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA diDetail, WiiControllerFoundDelegate wiiControllerFound)
        {
            // Get ProductName
            string product = string.Empty;
            byte[] productBuff = new byte[128];
            // attempt to read the serial number string
            if (HIDImports.HidD_GetProductString(handle.DangerousGetHandle(), productBuff, productBuff.Length))
            {
                // convert from unicode to the default encoding
                product = Encoding.Default.GetString(Encoding.Convert(Encoding.Unicode, Encoding.Default, productBuff));
            }
            // Trim the string down by removing any '\0' characters
            product = product.Remove(product.IndexOf('\0'));

            switch (controllerType)
            {
                case ControllerType.WiiBalanceBoard:
                    if (product == WBBName_new || product == WBBName_old)
                    {
                        if (wiiControllerFound != null)
                        {
                            wiiControllerFound(diDetail.DevicePath, controllerType);
                        }
                        return true;
                    }
                    break;
                default:
                    break;
            }

            return false;
        }

        private void WiiControllerFound(string devicePath, ControllerType controllerType)
        {
            WiiController wiiController = null;
            switch (controllerType)
            {
                case ControllerType.WiiBalanceBoard:
                    wiiController = new BalanceBoard();
                    break;
                default:
                    throw new WiiControllerException("Not supported ControllerType!");
            }
            wiiController.OpenWiiControllerDeviceHandle(devicePath);
            if (wiiController != null)
            {
                collection.Add(wiiController);
            }
        }

        internal string SearchWiiController(ControllerType controllerType)
        {
            int index = 0;
            Guid guid;
            SafeFileHandle handle;

            // get the GUID of the HID class
            HIDImports.HidD_GetHidGuid(out guid);

            // get a handle to all devices that are part of the HID class
            // Fun fact:  DIGCF_PRESENT worked on my machine just fine.  I reinstalled Vista, and now it no longer finds the WiiController with that parameter enabled...
            IntPtr hDevInfo = HIDImports.SetupDiGetClassDevs(ref guid, null, IntPtr.Zero, HIDImports.DIGCF_DEVICEINTERFACE);// | HIDImports.DIGCF_PRESENT);

            // create a new interface data struct and initialize its size
            HIDImports.SP_DEVICE_INTERFACE_DATA diData = new HIDImports.SP_DEVICE_INTERFACE_DATA();
            diData.cbSize = Marshal.SizeOf(diData);

            // get a device interface to a single device (enumerate all devices)
            while (HIDImports.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref guid, index, ref diData))
            {
                UInt32 size;

                // get the buffer size for this device detail instance (returned in the size parameter)
                HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, IntPtr.Zero, 0, out size, IntPtr.Zero);

                // create a detail struct and set its size
                HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA diDetail = new HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA();

                // on Win x86, cbSize must be 5 for some reason.  On x64, apparently 8 is what it wants.
                diDetail.cbSize = (uint)(IntPtr.Size == 8 ? 8 : 5);

                // actually get the detail struct
                if (HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, ref diDetail, size, out size, IntPtr.Zero))
                {
                    Debug.WriteLine(string.Format("{0}: {1} - {2}", index, diDetail.DevicePath, Marshal.GetLastWin32Error()));

                    // open a read/write handle to our device using the DevicePath returned
                    handle = HIDImports.CreateFile(diDetail.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);

                    // create an attributes struct and initialize the size
                    HIDImports.HIDD_ATTRIBUTES attrib = new HIDImports.HIDD_ATTRIBUTES();
                    attrib.Size = Marshal.SizeOf(attrib);

                    // get the attributes of the current device
                    if (HIDImports.HidD_GetAttributes(handle.DangerousGetHandle(), ref attrib))
                    {
                        // if the vendor and product IDs match up
                        if (attrib.VendorID == WiiInputManager.VID)
                        {
                            if (attrib.ProductID == WiiInputManager.PID_old || attrib.ProductID == WiiInputManager.PID_new)
                            {
                                if (CheckWiiControllerType(handle, controllerType, diDetail, null))
                                {
                                    // it's the right WiiController
                                    Debug.WriteLine("Found one!");

                                    return diDetail.DevicePath;
                                }
                            }
                        }
                    }
                    handle.Close();
                }
                else
                {
                    // failed to get the detail struct
                    throw new WiiControllerException("SetupDiGetDeviceInterfaceDetail failed on index " + index);
                }

                // move to the next device
                index++;
            }

            // clean up our list
            HIDImports.SetupDiDestroyDeviceInfoList(hDevInfo);

            // if we didn't find a WiiController, throw an exception
            throw new WiiControllerNotFoundException("No WiiController found in HID device list.");
        }

        #endregion
    }
}
