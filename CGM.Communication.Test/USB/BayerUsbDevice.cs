
using CGM.Communication.Interfaces;
using CGM.Win.HidDevice;
using CGM.Win.HidDevice.Win32;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CGM.Communication.Test.USB
{

    public class BayerUsbDevice : HIDDevice, IDevice
    {

        bool _isConnected = true;
        public bool IsConnected { get { return _isConnected; } set { _isConnected = value; } }

        public event DataReceivedEventHandler DataReceived;
        public static BayerUsbDevice FindMeter()
        {
            int _vendorIdFilter = 0x1a79;
            int _productIdFilter = 0x6210;
            return FindDevice<BayerUsbDevice>(_vendorIdFilter, _productIdFilter);
        }


        public async Task SendBytes(byte[] command)
        {
            MeterOutputReport oRep = new MeterOutputReport(this);

            oRep.SendCommand(command);
            try
            {
                Write(oRep);
            }
            catch (Exception)
            {
                // Device may have been removed!
            }
            //return oRep;
        }

        protected override void HandleDataReceived(InputReport oInRep)
        {
            DataReceived?.Invoke(this, oInRep.Buffer);
        }

        protected override InputReport CreateInputReport()
        {
            return new MeterInputReport();
        }

        protected override void HandleDeviceRemoved()
        {
            //throw new NotImplementedException();
        }
    }






}
