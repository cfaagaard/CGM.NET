using CGM.Communication.Common.Serialize;
using CGM.Communication.Interfaces;
using CGM.Communication.Log;
using CGM.Communication.MiniMed.Responses;
using CGM.Communication.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Power;
using System.Linq;
using CGM.Uwp.Models;

namespace CGM.Uwp.Tasks
{
    public class Nightscout : NightScoutTask
    {
        public Nightscout():base()
        {
            Messenger.Default.Register<BayerUsbDevice>(this, (device) => Disconnect(device));
        }

        protected override void GotSession(SerializerSession session)
        {
            base.GotSession(session);
            if (session != null && session.Status.Count > 0)
            {
                Messenger.Default.Send<SerializerSession>(session);
            }
        }

        public void Disconnect(BayerUsbDevice device)
        {
            if (!device.IsConnected)
            {
                if (this._tokenSource != null)
                {
                    this._tokenSource.Cancel();
                    this.Stop();
                }
            }
          
            
        }
    }
}
