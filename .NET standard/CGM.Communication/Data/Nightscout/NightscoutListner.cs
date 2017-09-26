//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNet.SignalR.Client;
//namespace CGM.Communication.Data.Nightscout
//{
//    public class NightscoutListner
//    {
//        private string _url;
//        private string _name;
//        public NightscoutListner(string url,string name)
//        {
//            _url = url;
//            _name = name;
//        }

//        public async Task Init()
//        {
//            var hubConnection = new HubConnection(_url);
//            var hubProxy = hubConnection.CreateHubProxy(_name);
//            hubProxy.On
//        }
//    }
//}
