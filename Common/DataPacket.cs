using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
        [MethodImpl(MethodImplOptions.Synchronized)]
        static public DataPacket DeserializeAddPacket(byte[] data)      //TODO: correggo il problema
                                                                        //quando invio i dati via muticast l'ultimo pacchetto ha un padding che non vuol dire niente MA viene considerato per il pacchetto successivo
                                                                        //trovare un modo per cancellare quel padding in ricezione od inserirlo nella classe di invio per identificarlo in ricezione
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
                byte[] buf = DeserializeServiceMemory.GetBuffer();

                
                int numberOfBytesToRemove = ((int)DeserializeServiceMemory.Position/1000 +1)*1000;
                if (numberOfBytesToRemove > DeserializeServiceMemory.Length)
                    numberOfBytesToRemove = (int)DeserializeServiceMemory.Length;
                Buffer.BlockCopy(buf, numberOfBytesToRemove, buf, 0, (int)DeserializeServiceMemory.Length - numberOfBytesToRemove);
                DeserializeServiceMemory.SetLength(DeserializeServiceMemory.Length - numberOfBytesToRemove);


                return dp;
            }
            catch (Exception e)
            {
                DeserializeServiceMemory.Seek(pos, SeekOrigin.Begin);
            }
            finally
            {
                //DeserializeServiceMemory.Seek(pos, SeekOrigin.Begin);
            }
            return null;
        }


        public delegate void DeserializationCompleteDelegate(DataPacket dp);
        public static event DeserializationCompleteDelegate onDeserializationComplete;

    }

   
}
