using System;
using System.Runtime.InteropServices;

namespace CGM.Win.HidDevice.Win32
{
    /// <summary>
    /// Generic HID device exception
    /// </summary>
    public class HIDDeviceException : Exception
    {
        public HIDDeviceException(string strMessage) : base(strMessage) {}

        public static HIDDeviceException GenerateWithWinError(string strMessage)
        {
            return new HIDDeviceException(string.Format("Msg:{0} WinEr:{1:X8}", strMessage, Marshal.GetLastWin32Error()));
        }
    }
}
