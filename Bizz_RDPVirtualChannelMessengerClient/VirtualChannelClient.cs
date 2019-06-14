using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Bizz_RDPVirtualChannelMessengerClient.WtsApi32;

namespace Bizz_RDPVirtualChannelMessengerClient
{
    class VirtualChannelClient
    {
        static IntPtr Channel;
        static ChannelEntryPoints EntryPoints;
        static int OpenChannel = 0;
        const string channelName = "BizzVC";

        static ChannelInitEventDelegate channelInitEventDelegate = new ChannelInitEventDelegate(VirtualChannelInitEventProc);
        static ChannelOpenEventDelegate channelOpenEventDelegate = new ChannelOpenEventDelegate(VirtualChannelOpenEvent);

        // https://docs.microsoft.com/en-us/windows/desktop/termserv/virtual-channel-client-dll 
        [DllExport]
        public static bool VirtualChannelEntry(ref ChannelEntryPoints entry)
        {
            ChannelDef[] channeldefinition = new ChannelDef[1];
            channeldefinition[0] = new ChannelDef();
            EntryPoints = entry;

            channeldefinition[0].name = channelName;
            ChannelReturnCodes ret = EntryPoints.VirtualChannelInit(ref Channel, channeldefinition, 1, 1, channelInitEventDelegate);

            if (ret != ChannelReturnCodes.Ok)
            {
                // Failed call of VirtualChannelEntry
                string[] output = { $"Failed VirtualChannelEntry - {ret.ToString()} : " + DateTime.Now.ToUniversalTime() };
                System.IO.File.WriteAllLines(@"C:\Users\luke.schofield\Desktop\Output.txt", output);
            }
            else
            {
                //Success call of VirtualChannelEntry
                string[] output = { $"Success VirtualChannelEntry - {ret.ToString()} : " + DateTime.Now.ToUniversalTime() };
                System.IO.File.WriteAllLines(@"C:\Users\luke.schofield\Desktop\Output.txt", output);
            }
            return true;
        }

        private static void VirtualChannelInitEventProc(IntPtr initHandle, ChannelEvents Event, byte[] data, int dataLength)
        {
            string[] output = { };
            switch (Event)
            {
                case ChannelEvents.Initialized:
                    output = new string[] { $"Channel initialised. : " + DateTime.Now.ToUniversalTime() };
                    System.IO.File.WriteAllLines(@"C:\Users\luke.schofield\Desktop\Output.txt", output);
                    break;
                case ChannelEvents.Connected:
                    output = new string [] { $"Channel connected. Calling to channel open event delegate : " + DateTime.Now.ToUniversalTime() };
                    System.IO.File.WriteAllLines(@"C:\Users\luke.schofield\Desktop\Output.txt", output);
                    ChannelReturnCodes ret = EntryPoints.VirtualChannelOpen(initHandle, ref OpenChannel, channelName, channelOpenEventDelegate);
                    break;
                case ChannelEvents.V1Connected:
                    output = new string[] { $"v1 connected. : " + DateTime.Now.ToUniversalTime() };
                    System.IO.File.WriteAllLines(@"C:\Users\luke.schofield\Desktop\Output.txt", output);
                    break;
                case ChannelEvents.Disconnected:
                    output = new string[] { $"Disconnected. : " + DateTime.Now.ToUniversalTime() };
                    System.IO.File.WriteAllLines(@"C:\Users\luke.schofield\Desktop\Output.txt", output);
                    break;
                case ChannelEvents.Terminated:
                    output = new string[] { $"Terminated. : " + DateTime.Now.ToUniversalTime() };
                    System.IO.File.WriteAllLines(@"C:\Users\luke.schofield\Desktop\Output.txt", output);
                    break;
            }
        }
        private static void VirtualChannelOpenEvent(int openHandle, ChannelEvents Event, byte[] data, int dataLength, uint totalLength, ChannelFlags dataFlags)
        {
            string[] output = new string[] { $"Channel Open Event Fired. This is good news!!! : " + DateTime.Now.ToUniversalTime(), Event.ToString(), dataFlags.ToString(),
                                            $"Data received through Virtual Channel : {Encoding.ASCII.GetString(data)}" };
            System.IO.File.WriteAllLines(@"C:\Users\luke.schofield\Desktop\Output.txt", output);
        }
    }

}
