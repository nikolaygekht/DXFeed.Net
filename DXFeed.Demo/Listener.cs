using DXFeed.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXFeed.Demo
{
    internal class Listener : IDXFeedConnectionListener
    {
        public void OnException(IDXFeedConnection connection, Exception exception)
        {
            Console.WriteLine("Exception in communicator: {0}", exception.Message); 
        }

        public void OnStatusChanged(IDXFeedConnection connection, DXFeedConnectionState state)
        {
            Console.WriteLine("Status changed: {0}", state);
        }
    }
}
