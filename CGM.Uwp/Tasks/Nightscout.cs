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
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using System.Net.NetworkInformation;

namespace CGM.Uwp.Tasks
{
    public class Nightscout : NightScoutTask
    {

        private WiFiAdapter firstAdapter;
        private string savedProfileName = null;
        private ConnectionProfile connectedProfile;
        private string nettype="(None)";
        public Nightscout() : base()
        {
            Messenger.Default.Register<BayerUsbDevice>(this, (device) => Disconnect(device));
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            CheckNet();
        }

        protected override void GotSession(SerializerSession session)
        {
            base.GotSession(session);
            if (session != null) //&& session.Status.Count > 0
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
                    NetworkChange.NetworkAddressChanged -= NetworkChange_NetworkAddressChanged;
                    this.Stop();
                }
            }


        }

        protected override bool CheckNet()
        {

            if (!IsInternet())
            {
                Logger.LogInformation($"(No Internet)");
                return false;
            }
            else
            {
                Task.Run(() => GetNet()).Wait();

                return true;
            }
        
            

        }

        protected override int GetBattery()
        {

            var aggBattery = Battery.AggregateBattery;
            var report = aggBattery.GetReport();
            if (report.RemainingCapacityInMilliwattHours.HasValue)
            {
                var getPercentage = (report.RemainingCapacityInMilliwattHours.Value / (double)report.FullChargeCapacityInMilliwattHours.Value);
                return Convert.ToInt32((Math.Round(getPercentage, 2) * 100));
            }
            //no battery -> raspberry pi 
            return 100;
        }
        //Here savedProfileName will have network ssid its connected.
        //Also connectedProfile.IsWlanConnectionProfile will be true if connected over wifi
        //connectedProfile.IsWwanConnectionProfile will be true if connected over cellular 
        private async Task GetNet()
        {
            var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
            if (result.Count >= 1)
            {
                firstAdapter = await WiFiAdapter.FromIdAsync(result[0].Id);

                if (firstAdapter.NetworkAdapter.GetConnectedProfileAsync() != null)
                {
                    connectedProfile = await firstAdapter.NetworkAdapter.GetConnectedProfileAsync();
                    if (connectedProfile != null) //&& !connectedProfile.ProfileName.Equals(savedProfileName)
                    {
                        savedProfileName = connectedProfile.ProfileName;

                        if (connectedProfile.IsWlanConnectionProfile)
                        {
                            nettype = $"Wifi \"{savedProfileName}\"";
                        }
                        if (connectedProfile.IsWwanConnectionProfile)
                        {
                            nettype = "cellular";
                        }
                        Logger.LogInformation($"Connected to internet: {nettype}. Battery: {GetBattery()}%");

                    }
                    else
                    {
                        Logger.LogInformation($"No connectedProfile");
                    }
                }
                else
                {
                    Logger.LogInformation($"No network-profil");
                }
            }
            else
            {
                Logger.LogInformation($"No network-devices");
            }
        }

        public bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
           
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }

        protected override void ResetCommunication(SerializerSession session)
        {
            base.ResetCommunication(session);
           Task.Run(()=> Windows.ApplicationModel.Core.CoreApplication.RequestRestartAsync(""));
        }
    }
}
