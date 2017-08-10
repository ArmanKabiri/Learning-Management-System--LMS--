using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SilverlightSignalRTest.Web
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        public void SendMessage(string name, string message)
        {
            Clients.All.received(name, message);
        }

        public void SendWholePacket(string wholePacket)
        {
            Clients.All.receivedWholePacket(wholePacket);
        }
    }

    [HubName("ChatHub2")]
    public class ChatHub2 : Hub
    {
        public void SendMessage(string name, string message)
        {
            Clients.All.received(name, message);
        }

        public void SendWholePacket(string wholePacket)
        {
            Clients.All.receivedWholePacket(wholePacket);
        }
    }

    [HubName("ChatHub3")]
    public class ChatHub3 : Hub
    {
        public void SendMessage(string name, string message)
        {
            Clients.All.received(name, message);
        }

        public void SendWholePacket(string wholePacket)
        {
            Clients.All.receivedWholePacket(wholePacket);
        }
    }

    [HubName("ChatHub4")]
    public class ChatHub4 : Hub
    {
        public void SendMessage(string name, string message)
        {
            Clients.All.received(name, message);
        }

        public void SendWholePacket(string wholePacket)
        {
            Clients.All.receivedWholePacket(wholePacket);
        }
    }

    [HubName("ChatHub5")]
    public class ChatHub5 : Hub
    {
        public void SendMessage(string name, string message)
        {
            Clients.All.received(name, message);
        }

        public void SendWholePacket(string wholePacket)
        {
            Clients.All.receivedWholePacket(wholePacket);
        }
    }

    [HubName("ChatHub6")]
    public class ChatHub6 : Hub
    {
        public void SendMessage(string name, string message)
        {
            Clients.All.received(name, message);
        }

        public void SendWholePacket(string wholePacket)
        {
            Clients.All.receivedWholePacket(wholePacket);
        }
    }

    [HubName("ChatHub7")]
    public class ChatHub7 : Hub
    {
        public void SendMessage(string name, string message)
        {
            Clients.All.received(name, message);
        }

        public void SendWholePacket(string wholePacket)
        {
            Clients.All.receivedWholePacket(wholePacket);
        }
    }

}