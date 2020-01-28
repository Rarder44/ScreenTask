using ExtendCSharp.Classes;
using ExtendCSharp.ExtendedClass;
using ExtendCSharp.TOFIX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Classes
{
    class Receiver_TCP : IReceiverServices
    {
        public event DataReceivedEventHandler OnDataReceived;
        public event EventHandler OnError;

        TcpClientPlus connection;
        private string DestinationIpAddress;
        private int DestinationPort;

        bool Connesso = false;

        public void Setup(string SourceIpAddress, int? SourcePort, string DestinationIpAddress, int? DestinationPort)
        {
            if (DestinationIpAddress == null)
                this.DestinationIpAddress = "127.0.0.1";
            else
                this.DestinationIpAddress = DestinationIpAddress;


            if (DestinationPort == null)
                this.DestinationPort = 0;
            else
                this.DestinationPort = DestinationPort.Value;


            //SourceIpAddress e SourcePort non servono
        }

        public async void Start()
        {
            try
            {           
                connection = await TcpClientPlus.Create(DestinationIpAddress, DestinationPort);
                connection.Closed += (TcpClientPlus client, out bool ToDispose) =>{
                    Stop();
                    ToDispose = true;
                };

                Connesso = true;
                new Task(TaskGetImage).Start();
                connection.StartCheckClose();
                
                Common.Log("Connessione effettuata");

            }
            catch (Exception ex)
            {
                Common.Log("Impossibile stabilire la connessione");
                Stop();
            }

        }

        private void TaskGetImage()
        {
            while (Connesso)        
            {
                DataPacket p = DataPacket.DeserializeFromStream(connection.GetStream());
                if (p == null)  //in caso ritorni null, ci puo essere un errore di comunicazione o la connessione è saltata
                {
                    CheckConnection(); //controllo la connessione
                }
                else
                {
                    OnDataReceived?.Invoke(p.Data, null);                  
                    
                }
            }
        }

        private void CheckConnection()
        {
            if (!connection.Connected)
            {
                Stop();       //se non c'è piu connessione, interrompo il ciclo di lettura
            }
        }


        public void Stop()
        {
            Connesso = false;
            OnError?.Invoke(this, new EventArgs());
        }
    }
}
