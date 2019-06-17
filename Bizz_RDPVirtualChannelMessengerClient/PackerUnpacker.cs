using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bizz_RDPVirtualChannelMessengerClient
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

        public Object UnpackObjectFromByteArray(byte[] b)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binFormat = new BinaryFormatter();
                memStream.Write(b, 0, b.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                Object obj = (Object)binFormat.Deserialize(memStream);
                return obj;
            }
        }

        public TransferObj CastObjectToTransferObj(object o)
        {
            try
            {
                return (TransferObj)o;
            }
            catch
            {
                return null;
            }
        }
    }
}
