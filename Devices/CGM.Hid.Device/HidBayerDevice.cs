

using CGM.Communication.Interfaces;
using CGM.Communication.Log;
using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Encodings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CGM.Communication.Extensions;
using System.IO;
using System.Text;
using CGM.Communication.Common;

namespace CGM.Hid.Device
{

    public class HidBayerDevice : DeviceBase
    {
        private int _vendorIdFilter = Constants.VendorIdFilter;// 0x1a79;
        private int _productIdFilter = Constants.ProductIdFilter;// 0x6210;
        private ILogger Logger = ApplicationLogging.CreateLogger<HidBayerDevice>();
        public HidDevice Device { get; private set; }


        private DeviceList _list;



        public HidBayerDevice()
        {
            Start();
        }



        public override void SendBytes(List<List<byte>> messages)
        {
            HidStream hidStream;

            if (Device != null && Device.TryOpen(out hidStream))
            {
                hidStream.ReadTimeout = 10000;


                using (hidStream)
                {

                    var inputReportBuffer = new byte[Device.GetMaxInputReportLength()];
                    foreach (var message in messages)
                    {
                        hidStream.Write(message.ToArray(), 0, message.Count);
                    }

                    IAsyncResult ar = null;
                    while (true)
                    {
                        if (ar == null)
                        {
                            ar = hidStream.BeginRead(inputReportBuffer, 0, inputReportBuffer.Length, null, null);
                        }
                        if (ar != null)
                        {

                            if (ar.IsCompleted)
                            {
                                int byteCount = hidStream.EndRead(ar);
                                ar = null;
                                if (byteCount > 0)
                                {
                                    var bytes = inputReportBuffer.Take(byteCount).ToArray();
                                    OnDataReceived(bytes);
                                }
                            }
                            else
                            {
                                ar.AsyncWaitHandle.WaitOne(1000);
                            }
                        }
                        if (DoneReceived)
                        {
                            break;
                        }
                    }

                }
            }
            return;

        }

        public override void Dispose()
        {
            //USBStream = null;
            //result = null;
        }

        public void Start()
        {
            _list = DeviceList.Local;
            _list.Changed += List_Changed;
            FindBayer();
        }

        private void List_Changed(object sender, DeviceListChangedEventArgs e)
        {
            FindBayer();
        }

        private void FindBayer()
        {
            var l3 = _list.GetHidDevices();
            var bayerDevices = _list.GetHidDevices(_vendorIdFilter, _productIdFilter)?.ToList();

            if (bayerDevices != null && bayerDevices.Count == 1)
            {
                Device = bayerDevices[0];
                IsConnected = true;
                Logger.LogInformation($"Device found: {Device.GetFriendlyName()}");
                Thread.Sleep(3000);

                OnDeviceAdded();
            }
            else
            {

                if (Device != null)
                {
                    IsConnected = false;
                    Device = null;
                    Logger.LogInformation($"Device disconnected");
                    OnDeviceRemoved();
                }

            }
        }
    }
}
