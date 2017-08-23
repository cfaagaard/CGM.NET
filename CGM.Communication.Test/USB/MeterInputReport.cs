
using CGM.Communication.Interfaces;
using CGM.Win.HidDevice.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Test.USB
{
    public class MeterInputReport : InputReport
    {
        public string AstmFrame { get; set; }
        public override void ProcessData()
        {
            this.AstmFrame = System.Text.Encoding.UTF8.GetString((byte[])(object)Buffer, 0, BufferLength);
        }
    }
}
