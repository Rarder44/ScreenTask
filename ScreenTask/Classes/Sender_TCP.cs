using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTask.Classes
{
    class Sender_TCP :ISenderServices
    {
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

        public void Send(byte[] data)
        {
            //TODO: scorro tutti i client ed invio il dato 
            //TODO: pensare ad implementare un'ACK del dato con controllo di flusso
        }

        public void Start()
        {
           //TODO: faccio partire il thread di ascolto dei nuovi client
        }

        public void Stop()
        {
            //TODO: fermo il thread di ascolto 
        }

        public void Setup(int? SourcePort, int? DestinationPort, string SourceIP)
        {
            
        }
    }
}
