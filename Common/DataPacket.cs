using System;
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



        public void SerializeToStream(Stream s)
        {
            BinaryFormatter formatter = new BinaryFormatter(); // the formatter that will serialize my object on my stream 

            formatter.Serialize(s, this); // the serialization process 
        }

        public void DeserializeFromStream(Stream s)
        {
            BinaryFormatter formatter = new BinaryFormatter(); // the formatter that will serialize my object on my stream 
            DataPacket dp = (DataPacket)formatter.Deserialize(s);
            this.Data = dp.Data;
        }
    }
}
