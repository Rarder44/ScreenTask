using ExtendCSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTask.Classes
{
    interface ISenderServices :IService
    {

        void Init();
        void Send(byte[] data);

        void Start();
        void Stop();

        /// <summary>
        /// Permette di impostare i parametri per la connessione ( da richiamare prima dello start ) 
        /// Passare NULL per impostare il parametro di default
        /// </summary>
        /// <param name="SourcePort"></param>
        /// <param name="DestinationPort"></param>
        /// <param name="SourceIP"></param>
        void Setup(int? SourcePort, int? DestinationPort, String SourceIP);


    }
}
