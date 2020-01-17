using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Classes
{
    interface IReceiverServices
    {
        void Start();
        void Stop();
        void Setup(String SourceIpAddress,int? Listening_SourcePort, String DestinationIpAddress, int? DestinationPort );

        event DataReceivedEventHandler OnDataReceived;
        event EventHandler OnError;
    }
    public delegate void DataReceivedEventHandler(byte[] data, System.Net.EndPoint remoteEP);
}
