using Bizz_RDPVirtualChannelUtility;
using System;
using System.Runtime.InteropServices;
using System.Text;
using static Bizz_RDPVirtualChannelMessengerClient.WtsApi32;

namespace Bizz_RDPVirtualChannelMessengerClient
{
    
    class VirtualChannelClient
    {
        // SET BOOL TO TRUE IF USING STRING TRANSFER AND NOT OBJECT TRANSFER [Testing Purposes]
        static bool usingString = false;


        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        static IntPtr Channel;
        static ChannelEntryPoints EntryPoints;
        static int OpenChannel = 0;
        const string channelName = "BizzVC";
        const string outputAddress = @"C:\Users\luke.schofield\Desktop\Output.txt";

        static ChannelInitEventDelegate channelInitEventDelegate = new ChannelInitEventDelegate(VirtualChannelInitEventProc);
        static ChannelOpenEventDelegate channelOpenEventDelegate = new ChannelOpenEventDelegate(VirtualChannelOpenEvent);

        // https://docs.microsoft.com/en-us/windows/desktop/termserv/virtual-channel-client-dll 
        [DllExport]
        public static bool VirtualChannelEntry(ref ChannelEntryPoints entry)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            try
            {
                ChannelDef[] channeldefinition = new ChannelDef[1];
                channeldefinition[0] = new ChannelDef();
                EntryPoints = entry;

                channeldefinition[0].name = channelName;
                ChannelReturnCodes ret = EntryPoints.VirtualChannelInit(ref Channel, channeldefinition, 1, 1, channelInitEventDelegate);

                AllocConsole();
                Console.WriteLine("Console Created!.");

                if (ret != ChannelReturnCodes.Ok)
                {
                    // Failed call of VirtualChannelEntry
                    string[] output = { $"Failed VirtualChannelEntry - {ret.ToString()} : " + DateTime.Now.ToUniversalTime() };
                    System.IO.File.WriteAllLines(outputAddress, output);
                    foreach (var s in output) { Console.WriteLine(s); }
                }
                else
                {
                    //Successful call of VirtualChannelEntry
                    string[] output = { $"Success VirtualChannelEntry - {ret.ToString()} : " + DateTime.Now.ToUniversalTime() };
                    System.IO.File.WriteAllLines(outputAddress, output);
                    foreach (var s in output) { Console.WriteLine(s); }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"virtualchannelEntry : {e.ToString()}");
                return false;
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"DDDang it! {e.ExceptionObject.ToString()} T");
            Console.ReadKey();
        }

        private static void VirtualChannelInitEventProc(IntPtr initHandle, ChannelEvents Event, byte[] data, int dataLength)
        {
            try
            {
                string[] output = { };
                switch (Event)
                {
                    case ChannelEvents.Initialized:
                        output = new string[] { $"Channel initialised. : " + DateTime.Now.ToUniversalTime() };
                        System.IO.File.WriteAllLines(outputAddress, output);
                        foreach (var s in output) { Console.WriteLine(s); }
                        break;
                    case ChannelEvents.Connected:
                        output = new string[] { $"Channel connected. Calling to channel open event delegate : " + DateTime.Now.ToUniversalTime() };
                        System.IO.File.WriteAllLines(outputAddress, output);
                        foreach (var s in output) { Console.WriteLine(s); }
                        ChannelReturnCodes ret = EntryPoints.VirtualChannelOpen(initHandle, ref OpenChannel, channelName, channelOpenEventDelegate);
                        break;
                    case ChannelEvents.V1Connected:
                        output = new string[] { $"v1 connected. : " + DateTime.Now.ToUniversalTime() };
                        System.IO.File.WriteAllLines(outputAddress, output);
                        foreach (var s in output) { Console.WriteLine(s); }
                        break;
                    case ChannelEvents.Disconnected:
                        output = new string[] { $"Disconnected. : " + DateTime.Now.ToUniversalTime() };
                        System.IO.File.WriteAllLines(outputAddress, output);
                        foreach (var s in output) { Console.WriteLine(s); }
                        break;
                    case ChannelEvents.Terminated:
                        output = new string[] { $"Terminated. : " + DateTime.Now.ToUniversalTime() };
                        System.IO.File.WriteAllLines(outputAddress, output);
                        foreach (var s in output) { Console.WriteLine(s); }
                        break;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($" THIS IS VirtualChannelInitEventProc : {e.ToString()}");
                Console.ReadKey();
            }
        }
        private static void VirtualChannelOpenEvent(int openHandle, ChannelEvents Event, byte[] data, int dataLength, uint totalLength, ChannelFlags dataFlags)
        {
            try
            {
                PackerUnpacker utility = new PackerUnpacker();
                switch (Event)
                {
                    case ChannelEvents.DataRecived:
                        {
                            string[] output;
                            try
                            {
                                if (usingString)
                                {
                                    output = new string[] { $"Channel Open Event Fired : {Event.ToString()} : " + DateTime.Now.ToUniversalTime(),
                                                        $"Data received through Virtual Channel : {Encoding.ASCII.GetString(data)}" + Environment.NewLine };
                                }
                                else
                                {
                                    var receivedData = utility.UnpackObjectFromByteArray(data);
                                    output = new string[] {$"Channel Open Event Fired: {Event.ToString()} : " + DateTime.Now.ToUniversalTime(),
                                                        $"Data received through Virtual Channel : " +
                                                        $"{ (receivedData == null ? "Nothing there" : $"{receivedData.Name}") }" } ;
                                                         //$"{utility.CastObjectToTransferObj(utility.UnpackObjectFromByteArray(data)).Name}" };
                                }

                                System.IO.File.WriteAllLines(outputAddress, output);
                                foreach (var s in output) { Console.WriteLine(s); }
                                Console.WriteLine(Environment.NewLine);
                                Console.WriteLine("Please enter your response: ");
                                var response = Console.ReadLine();
                                if (usingString)
                                {
                                    EntryPoints.VirtualChannelWrite(openHandle, Encoding.ASCII.GetBytes(response), (uint)Encoding.ASCII.GetBytes(response).Length, IntPtr.Zero);

                                }
                                else
                                {
                                    TransferObj toSend = new TransferObj();
                                    toSend.Name = response;
                                    toSend.Value = 1;
                                    EntryPoints.VirtualChannelWrite(openHandle, utility.PackObjectToByteArray(toSend), (uint)utility.PackObjectToByteArray(toSend).Length, IntPtr.Zero);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                            break;
                        }
                    default:
                        {
                            string[] output = new string[] { $"Channel Open Event Fired : {Event.ToString()} : " + DateTime.Now.ToUniversalTime() };
                            foreach (var s in output) { Console.WriteLine(s); }
                            break;
                        }
                }
            }
            catch(Exception ALLOFTHEM)
            {
                Console.WriteLine($" VirtualChannelOpenEvent : { ALLOFTHEM.ToString() }");
                Console.ReadKey();
            }


        }

        
    }

}
