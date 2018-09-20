using CGM.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage.Streams;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using CGM.Communication.Log;
using System.Runtime.InteropServices.WindowsRuntime;
using CGM.Communication.MiniMed;
using CGM.Communication.Common;

namespace CGM.Uwp.Device
{


    public class UwpBayerDevice : DeviceBase
    {
        //private int _vendorIdFilter = 0x1a79;
        //private int _productIdFilter = 0x6210;
        private ushort usageId = 1;
        private ushort usagePage = 65344;

        private DeviceWatcher _inputWatcher;
        private ILogger Logger = ApplicationLogging.CreateLogger<UwpBayerDevice>();
        private HidDevice _device;

        #region properties

        private DeviceInformation _deviceInfo;

        public DeviceInformation DeviceInfo
        {
            get { return _deviceInfo; }
            set
            {
                _deviceInfo = value;
                OnPropertyChanged("DeviceInfo");
                this.IsConnected = _deviceInfo != null;
            }
        }

        private string _id;

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        #endregion

        public UwpBayerDevice()
        {
            StartListen();
        }

        
        //public async Task SendBytes(CommunicationBlock CommunicationBlock)
        //{
        //    var lists = CommunicationBlock.GetRequestBytes();
        //    foreach (var message in lists)
        //    {
        //        Logger.LogTrace(BitConverter.ToString(message.ToArray()));
        //        SendBytes(message.ToArray());
        //    }
        //}
        public override void SendBytes(List<List<byte>> messages)
        {
            foreach (var message in messages)
            {
                Logger.LogTrace(BitConverter.ToString(message.ToArray()));
                SendBytes(message.ToArray());
            }
            
        }

        public void SendBytes(byte[] message)
        {
            HidOutputReport outReport = _device.CreateOutputReport(0);

            DataWriter dataWriter = new DataWriter();
            dataWriter.WriteBytes(message);

            if (message.Length < outReport.Data.Capacity)
            {
                int repeat = ((int)outReport.Data.Capacity) - message.Length;

                IEnumerable<byte> bytes = Enumerable.Repeat<byte>(0, repeat);
                dataWriter.WriteBytes(bytes.ToArray());
            }
            outReport.Data = dataWriter.DetachBuffer();
            try
            {
                Task.Run(() => _device.SendOutputReportAsync(outReport)).Wait() ;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw;
            }




        }

        private void _device_InputReportReceived(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            OnDataReceived(args.Report.Data.ToArray());
        }

        public override void Dispose()
        {
            // throw new NotImplementedException();
        }

        private void StartListen()
        {
            string aqs = HidDevice.GetDeviceSelector(usagePage, usageId);

            _inputWatcher = Windows.Devices.Enumeration.DeviceInformation.CreateWatcher(aqs);
            _inputWatcher.Removed += InputWatcher_Removed;
            _inputWatcher.Added += InputWatcher_Added;
            _inputWatcher.Start();

        }

        private void InputWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {

            Logger.LogInformation($"Device disconnected");
            DeviceInfo = null;
            Id = "";
            Name = "";
            OnDeviceRemoved();
        }


        private void InputWatcher_Added(DeviceWatcher sender, Windows.Devices.Enumeration.DeviceInformation args)
        {
            SetDevice();
        }


        private async void SetDevice()
        {
            string aqs = HidDevice.GetDeviceSelector(usagePage, usageId);
            var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(aqs);
            foreach (var item in devices)
            {
                _device = await HidDevice.FromIdAsync(item.Id, Windows.Storage.FileAccessMode.ReadWrite);
                if (_device == null)
                {

                    //no--- no 2.4--"\\\\?\\HID#VID_1A79&PID_6300#6&33f175f6&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"
                    //yes-- 2.4 ----"\\\\?\\HID#VID_1A79&PID_6210#6&21757738&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"
                    //yes-- 2.4 ----"\\\\?\\HID#VID_1A79&PID_6210#6&c51bede&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"
                    if (item.Id.Contains("PID_6210"))
                    {
                        Logger.LogError($"Could not find device: {item.Id}");
                    }
                    else
                    {
                        Logger.LogError($"The USB device is not a correct version. Please use a 'Contour Link USB Device - version 2.4'.");
                    }
                    return;
                }
                else
                {
                    DeviceInfo = item;
                    Id = item.Id;
                    this.IsConnected = true;
                    Logger.LogInformation($"Device found: {DeviceInfo.Name}");
  
                    _device.InputReportReceived += _device_InputReportReceived;
                    OnDeviceAdded();
                    
                    break;
                }
            }


        }


    }
}
