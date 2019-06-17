using Bizz_RDPVirtualChannelMessenger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

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

        public object UnpackObjectFromByteArray(byte[] b)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binFormat = new BinaryFormatter();
                memStream.Write(b, 0, b.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                return (Object)binFormat.Deserialize(memStream);
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
