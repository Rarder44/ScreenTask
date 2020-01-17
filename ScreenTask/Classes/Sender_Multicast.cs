using ExtendCSharp.ExtendedClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTask.Classes
{
    class Sender_Multicast:ISenderServices
    {
        /// <summary>
        /// 0 = random port
        /// </summary>
        public int SourcePort { get; set; }// = 0;
        public String SourceIP { get; set; }// = "127.0.0.1";


        public int DestinationPort { get; set; }// = 7070;
        public String MulticastAddress { get; set; } = CommonLib.CommonSetting.MulticastAddress;
       
        MulticastClient sender;

        public Sender_Multicast()
        {
            Setup(null, null, null); //Setto i valori di default;
        }


        public void Init()
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] data)
        {
            sender.SendGroup(data, (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds());
        }

        

        public void Start()
        {
            sender = new MulticastClient(MulticastAddress, SourcePort,DestinationPort, SourceIP, false);
            sender.JoinMulticast();
            Common.Log("Server Started Successfuly!");
            Common.Log("Multicast destination : " + MulticastAddress + ":" + DestinationPort);
            Common.Log("Output interface: " + SourceIP);
           
        }

        public void Stop()
        {
            if (sender != null)
            {
                sender.Close();
                sender.Dispose();
            }
        }

        public void Setup(int? SourcePort, int? DestinationPort, string SourceIP)
        {
            if (SourcePort == null)
                this.SourcePort = 0;
            else
                this.SourcePort = SourcePort.Value;


            if (DestinationPort == null)
                this.DestinationPort = 7070;
            else
                this.DestinationPort = DestinationPort.Value;


            if (SourceIP == null)
                this.SourceIP = "127.0.0.1";
            else
                this.SourceIP = SourceIP;

        }
    }
}
