using ExtendCSharp.ExtendedClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Classes
{
    class Receiver_Multicast : IReceiverServices
    {
        public event DataReceivedEventHandler OnDataReceived;
        public event EventHandler OnError;

        MulticastClient c;
        private String MulticastAddress = CommonLib.CommonSetting.MulticastAddress;
        private string SourceIpAddress;
        private int Listening_SourcePort;


        public void Start()
        {
            if (c != null)
                c.Dispose();
            
            c = new MulticastClient(MulticastAddress, Listening_SourcePort, 0, SourceIpAddress);
            c.onReceivedByte += (byte[] data, System.Net.EndPoint remoteEP) =>
            {
                OnDataReceived?.Invoke(data, remoteEP);
            };

            c.StartListen();

        }

        public void Stop()
        {
            if (c != null)
            {
                c.StopListener();
                c.Dispose();
            }
        }

        public void Setup(string SourceIpAddress, int? Listening_SourcePort, string DestinationIpAddress, int? DestinationPort)
        {
            if (SourceIpAddress == null)
                this.SourceIpAddress = "127.0.0.1";
            else
                this.SourceIpAddress = SourceIpAddress;


            if (Listening_SourcePort == null)
                this.Listening_SourcePort = 0;
            else
                this.Listening_SourcePort = Listening_SourcePort.Value;


            //DestinationIpAddress e DestinationPort non servono
        }
    }
}
