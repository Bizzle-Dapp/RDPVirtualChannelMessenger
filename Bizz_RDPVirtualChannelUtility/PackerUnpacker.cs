using Bizz_RDPVirtualChannelUtility;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bizz_RDPVirtualChannelUtility
{
    public class PackerUnpacker
    {

        public byte[] PackObjectToByteArray(object o)
        {
            if ( o == null )
            {
                return null;
            }

            BinaryFormatter binFormat = new BinaryFormatter();
            using (MemoryStream memStream = new MemoryStream())
            {
                binFormat.Serialize(memStream, o);
                return memStream.ToArray();
            }
        }

        public TransferObj UnpackObjectFromByteArray(byte[] b)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binFormat = new BinaryFormatter();
                memStream.Write(b, 0, b.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                
                try
                {
                    var obj = binFormat.Deserialize(memStream);
                    return (TransferObj)obj;
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Failed to deserialize to TransferObj : {e}");
                    return null;
                }
                
            }
        }
    }
}
