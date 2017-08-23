using System;

namespace CGM.Win.HidDevice.Win32
{
    public interface IUsbEventService
    {
        void Register(IntPtr hwndHandle);
        bool Unregister(IntPtr hwndHandle);
    }
}
