using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace ServerHub
{
    [HubName("chat")]
    public class MyHub : Hub
    {
        [HubMethodName("announceToEverybody")]
        public void Announce(string message)
        {
            Clients.Others.Announce(message);
        }

        public DateTime GetServerDate()
        {
            return DateTime.Now;
        }
    }
}