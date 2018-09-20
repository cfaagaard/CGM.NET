using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CGM.Web.Hubs
{
    public class DataLoggerHub : Hub
    {
        protected IHubContext<DataLoggerHub> _context;

        List<string> messages = new List<string>();

        public DataLoggerHub(IHubContext<DataLoggerHub> context)
        {
            _context = context;
        }
        public async Task SendMessage(string message)
        {
            this.messages.Insert(0,message);
            await _context.Clients.All.SendAsync("ReceiveMessage",message);
        }

        public async Task SendStatus(string status)
        {
            await _context.Clients.All.SendAsync("ReceiveStatus", status);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
  // Clients.Caller.SendAsync()
        }
    }
}
