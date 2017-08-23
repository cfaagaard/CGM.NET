
using CGM.Communication.Interfaces;
using CGM.Win.HidDevice.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Win.HidDevice
{
    public class MeterOutputReport : OutputReport
    {

        public MeterOutputReport(HIDDevice oDev) : base(oDev)
        {
        }
        public void SendCommand(byte[] command)
        {
            for (int i = 0; i < command.Length; i++)
            {
                Buffer[i] = command[i];
            }
        }

    }

}
