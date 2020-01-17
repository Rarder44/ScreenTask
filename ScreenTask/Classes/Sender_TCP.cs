using ExtendCSharp.ExtendedClass;
using ExtendCSharp.TOFIX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTask.Classes
{
    class Sender_TCP :ISenderServices
    {

        List<TcpClientPlus> Clients = new List<TcpClientPlus>();
        TcpListenerPlus Listener = null;


        public int SourcePort { get; private set; }
        public int DestinationPort { get; private set; }
        public string SourceIP { get; private set; }




        public Sender_TCP()
        {
            
        }
      

        public void Init()
        {
            throw new NotImplementedException();
        }

        public async void Send(byte[] data)
        {
            DataPacket dp = new DataPacket();
            dp.Data = data;
            foreach (TcpClientPlus client in Clients.ToArray())
            {
                if (!client.Connected)
                    Client_disconnected(client);
                else
                {
                    try
                    {
                        dp.SerializeToStream(client.GetStream()); 
                    }
                    catch (Exception ex)
                    {
                        //errore nel'invio, client disconnesso -> rimuovo il client 
                        Client_disconnected(client);
                    }
                }


                //INVIARE I DATI IN MANIERA ASINCRONA!!!
            }
            //TODO: scorro tutti i client ed invio il dato 
            //TODO: pensare ad implementare un'ACK del dato con controllo di flusso
        }

        public void Start()
        {
            //TODO: faccio partire il thread di ascolto dei nuovi client
            Listener = new TcpListenerPlus(SourceIP, SourcePort);
            Listener.ClientConnected += Listener_ClientConnected;
            Listener.Start();
        }

        public void Stop()
        {
            //TODO: fermo il thread di ascolto 
            if (Listener != null)
                Listener.Stop();
            foreach (TcpClientPlus client in Clients)
                client.Close();
            Clients.Clear();
        }

        public void Setup(int? SourcePort, int? DestinationPort, string SourceIP)
        {
            if (SourcePort == null)
                this.SourcePort = 7070;
            else
                this.SourcePort = SourcePort.Value;


            if (DestinationPort == null)
                this.DestinationPort = 0;
            else
                this.DestinationPort = DestinationPort.Value;


            if (SourceIP == null)
                this.SourceIP = "127.0.0.1";
            else
                this.SourceIP = SourceIP;
        }


        private void Listener_ClientConnected(TcpClientPlus client)
        {
            Clients.Add(client);
        }
        private void Client_disconnected(TcpClientPlus client)
        {
            Clients.Remove(client);
        }
    }
}
