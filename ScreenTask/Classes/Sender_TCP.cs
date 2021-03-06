﻿using ExtendCSharp.ExtendedClass;
using ExtendCSharp.TOFIX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                {
                    ClientDisconnected(client);
                    Common.connectionsLog.RemoveConnection(new Forms.Connection((IPEndPoint)client.Client.RemoteEndPoint));
                }
                else
                {
                    try
                    {
                        //INVIARE I DATI IN MANIERA ASINCRONA!!!
                        dp.SerializeToStream(client.GetStream()).ContinueWith((previusTask) =>
                        {
                            IPEndPoint ep = (IPEndPoint)client.Client.RemoteEndPoint;
                            try
                            {
                                if (Common.connectionsLog != null && !Common.connectionsLog.IsDisposed)
                                    Common.connectionsLog.UpdateConnection(new Forms.Connection(ep.Address.ToString(), ep.Port, previusTask.Result.SpeedText));
                            }
                            catch (Exception e)
                            {
                                //errore dato dalla chiusura del form di log
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        //errore nel'invio, client disconnesso -> rimuovo il client 
                        ClientDisconnected(client);
                    }
                }
            }
            // scorro tutti i client ed invio il dato 
            //TODO: pensare ad implementare un'ACK del dato con controllo di flusso
        }

        public void Start()
        {
            //faccio partire il thread di ascolto dei nuovi client
            Listener = new TcpListenerPlus(SourceIP, SourcePort);
            Listener.ClientConnected += Listener_ClientConnected;
            Listener.Start();
            Common.Log("Server Started Successfuly!");
            Common.Log("TCP Listener : " + SourceIP + ":" + SourcePort);

        }

        public void Stop()
        {
            // fermo il thread di ascolto 
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
        private void ClientDisconnected(TcpClientPlus client)
        {
            Clients.Remove(client);
        }
    }
}
