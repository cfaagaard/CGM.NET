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
using GalaSoft.MvvmLight.Messaging;

namespace CGM.Uwp.Models
{



    //public class DisconnectedDevice
    //{

    //}
    //public class ConnectedDevice
    //{

    //}
    public class BayerUsbDevice : INotifyPropertyChanged, IDevice
    {
        //private int _vendorIdFilter = 0x1a79;
        //private int _productIdFilter = 0x6210;
        private ushort usageId = 1;
        private ushort usagePage = 65344;

        private DeviceWatcher _inputWatcher;
        private ILogger Logger = ApplicationLogging.CreateLogger<BayerUsbDevice>();
        private HidDevice _device;

        public event DataReceivedEventHandler DataReceived;
        public event PropertyChangedEventHandler PropertyChanged;


        #region properties


        private bool _isconnected;

        public bool IsConnected
        {
            get { return _isconnected; }
            set
            {
                _isconnected = value;
                OnPropertyChanged("IsConnected");
            }
        }


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

        public BayerUsbDevice()
        {
            StartListen();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public async Task SendBytes(byte[] message)
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
                await _device.SendOutputReportAsync(outReport);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw;
            }




        }

        private async Task Init()
        {

            _device = await HidDevice.FromIdAsync(Id, Windows.Storage.FileAccessMode.ReadWrite);
            if (_device == null)
            {
                throw new Exception($"Could not find device: {Id}");
            }

            _device.InputReportReceived += _device_InputReportReceived;
            
        }

        private void _device_InputReportReceived(HidDevice sender, HidInputReportReceivedEventArgs args)
        {

            DataReceived?.Invoke(this, args.Report.Data.ToArray());
        }

        public void Dispose()
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
            Messenger.Default.Send<BayerUsbDevice>(this);

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
                DeviceInfo = item;
                Id = item.Id;
                Logger.LogInformation($"Device found: {DeviceInfo.Name}");
                var initTask = Init();
                await Task.WhenAll(initTask);

                break;
            }
            await Task.Delay(3000);
            Messenger.Default.Send<BayerUsbDevice>(this);
        }

    }
}
