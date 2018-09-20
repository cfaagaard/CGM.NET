using CGM.Communication.MiniMed;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace CGM.Communication.Interfaces
{
    public delegate void DataReceivedEventHandler(object sender, byte[] data);

    public interface IDevice : IDisposable
    {
        void SendBytes(List<List<byte>> messages);
        event DataReceivedEventHandler DataReceived;
        bool IsConnected { get; set; }
        bool DoneReceived { get; set; }
        event EventHandler DeviceAdded;
        event EventHandler DeviceRemoved;
    }
}
