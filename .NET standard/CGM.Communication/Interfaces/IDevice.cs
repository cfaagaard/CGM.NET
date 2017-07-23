using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace CGM.Communication.Interfaces
{
    public delegate void DataReceivedEventHandler(object sender, byte[] data);

    public interface IDevice:IDisposable
    {
        Task SendBytes(byte[] message);
        event DataReceivedEventHandler DataReceived;
        bool IsConnected { get; set; }
    }
}
