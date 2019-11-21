﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public class DataPacket
    {
        public byte[] Data;



        /// <summary>
        /// Serializza l'oggetto in un array di byte
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter(); // the formatter that will serialize my object on my stream 
                formatter.Serialize(ms, this); // the serialization process 
                byte[] tmp = ms.ToArray();
                return tmp;
            }
        }
        async public Task SerializeToStream( Stream s)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter(); // the formatter that will serialize my object on my stream 
                formatter.Serialize(ms, this); // the serialization process 
                byte[] tmp = ms.ToArray();
                s.WriteAsync(tmp, 0, tmp.Length);  //NON AWAIT! non devo aspettare che termini!          
            }
            
        }
        static public DataPacket DeserializeFromStream(Stream s)
        {
            try
            {            
                BinaryFormatter formatter = new BinaryFormatter(); // the formatter that will serialize my object on my stream 
                DataPacket dp = (DataPacket)formatter.Deserialize(s);
                return dp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }








        static private MemoryStream DeserializeServiceMemory = new MemoryStream();
        
       /// <summary>
       /// Permette di deserializzare un oggetto dati dei byte NON provenienti da uno stream ( viene lanciato un evento quando il pacchetto è completato )
       /// </summary>
       /// <param name="data"></param>
        static public void DeserializeAddPacket(byte[] data)
        {
            long pos = DeserializeServiceMemory.Position;
            DeserializeServiceMemory.Write(data,0,data.Length);
            pos = DeserializeServiceMemory.Position;
            BinaryFormatter formatter = new BinaryFormatter(); 
            
            try
            {
               
                DeserializeServiceMemory.Seek(0, SeekOrigin.Begin);
                DataPacket dp = (DataPacket)formatter.Deserialize(DeserializeServiceMemory);  
                onDeserializationComplete?.Invoke(dp);
            }
            catch (Exception e)
            {
                DeserializeServiceMemory.Seek(pos, SeekOrigin.Begin);
            }
        }


        public delegate void DeserializationCompleteDelegate(DataPacket dp);
        public static event DeserializationCompleteDelegate onDeserializationComplete;

    }

   
}
