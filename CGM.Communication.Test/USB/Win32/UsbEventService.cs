using System;

namespace CGM.Win.HidDevice.Win32
{
    public class UsbEventService : IUsbEventService
    {
        public void Register(IntPtr hwnd)
        {
            Win32Usb.RegisterForUsbEvents(hwnd, Win32Usb.HIDGuid);
        }

        public bool Unregister(IntPtr hwnd)
        {
            return Win32Usb.UnregisterForUsbEvents(hwnd);
        }


        //public event EventHandler OnSpecifiedDeviceArrived;
        //public event EventHandler OnSpecifiedDeviceRemoved;

        //public event EventHandler OnDeviceArrived;
        //public event EventHandler OnDeviceRemoved;



        //    public void ParseMessages(ref Message m)
        //{
        //    if (m.Msg == Win32Usb.WM_DEVICECHANGE)  // we got a device change message! A USB device was inserted or removed
        //    {
        //        switch (m.WParam.ToInt32()) // Check the W parameter to see if a device was inserted or removed
        //        {
        //            case Win32Usb.DEVICE_ARRIVAL:   // inserted
        //                if (OnDeviceArrived != null)
        //                {
        //                    OnDeviceArrived(this, new EventArgs());
        //                }
        //                break;
        //            case Win32Usb.DEVICE_REMOVECOMPLETE:    // removed
        //                if (OnDeviceRemoved != null)
        //                {
        //                    OnDeviceRemoved(this, new EventArgs());
        //                }
        //                break;
        //        }
        //    }
        //}

    }
}
