using CGM.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CGM.Communication.Common
{
    public abstract class DeviceBase:IDevice
    {
        public event EventHandler DeviceAdded;
        public event EventHandler DeviceRemoved;

        public event DataReceivedEventHandler DataReceived;
        public event PropertyChangedEventHandler PropertyChanged;
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

        public bool DoneReceived { get; set; }


        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public abstract void SendBytes(List<List<byte>> messages);

        public abstract void Dispose();

        protected virtual void OnDeviceAdded()
        {
            DeviceAdded?.Invoke(this, new EventArgs());
        }
        protected virtual void OnDeviceRemoved()
        {
            DeviceRemoved?.Invoke(this, new EventArgs());
        }
        protected virtual void OnDataReceived(byte[] bytes)
        {
            DataReceived?.Invoke(this,bytes);
        }

    }
}
