using Bizz_RDPVirtualChannelUtility;
using System;
using System.Text;

namespace Bizz_RDPVirtualChannelMessengerServer
{
    
    class Program
    {
        static string channelName = "BizzVC";
        static void Main(string[] args)
        {
            Console.WriteLine("Please press enter to begin...");
            Console.ReadLine();
            // Open a virtual channel
            VirtualChannel.OpenVirtualChannel(channelName);
            Console.WriteLine("Connected!" + Environment.NewLine);

            DisplayChoice();

        }
        static void DisplayChoice()
        {
            Console.WriteLine($"Please select what you would like to do:{Environment.NewLine}" +
                                $"1     : Send String {Environment.NewLine}" +
                                $"2     : Read String  {Environment.NewLine}" + 
                                $"Space : Exit");
            var choice = Console.ReadKey().Key;
            switch (choice)
            {
                case ConsoleKey.D1:
                    {
                        Console.WriteLine($"Please enter text to send: {Environment.NewLine}");

                        //var ret = VirtualChannel.WriteChannel(Console.ReadLine());
                        TransferObj transferObj = new TransferObj();
                        transferObj.Name = Console.ReadLine();
                        transferObj.Value = 1;
                        var ret = VirtualChannel.WriteChannel(transferObj);
                        Console.WriteLine($"Write to channel returned: {ret}.");
                        Console.WriteLine("Press enter to continue...");
                        Console.ReadLine();
                        DisplayChoice();
                        break;
                    }
                case ConsoleKey.D2:
                    {
                        //Console.WriteLine(VirtualChannel.ReadChannelForString());
                        var ret = new TransferObj(); 
                        ret = VirtualChannel.ReadChannelForObject();
                        Console.WriteLine( (ret == null ? "Nothing there..." : ret.Name)  );
                        DisplayChoice();
                        break;
                    }
                case ConsoleKey.Spacebar:
                    {
                        Console.WriteLine($"Closing channel... Outcome : {VirtualChannel.CloseChannel()}");
                        Console.WriteLine("Press enter to close.");
                        Console.ReadLine();
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"Invalid input. {Environment.NewLine}{Environment.NewLine}{Environment.NewLine}");
                        DisplayChoice();
                        break;
                    }
            }
        }
    }

    public static class VirtualChannel
    {
        static IntPtr mHandle = IntPtr.Zero;
        public static void OpenVirtualChannel(string channelName)
        {
            mHandle = WtsApi32.WTSVirtualChannelOpen(IntPtr.Zero, -1, channelName);
        }

        public static string ReadChannelForString()
        {
            string output;
            byte[] data = new byte[1600];
            int read = 0;
            if(WtsApi32.WTSVirtualChannelRead(mHandle, 2000, data, data.Length, ref read))
            {
                output = Encoding.ASCII.GetString(data);
            }
            else
            {
                output = $"There is nothing on the channel to read... {Environment.NewLine}";
            }

            return output;
        }
        public static TransferObj ReadChannelForObject()
        {
            PackerUnpacker utility = new PackerUnpacker();
            TransferObj output = new TransferObj();
            byte[] data = new byte[1600];
            int read = 0;
            if(WtsApi32.WTSVirtualChannelRead(mHandle, 2000, data, data.Length, ref read))
            {
                output = utility.UnpackObjectFromByteArray(data);
                //TransferObj transferObj = utility.CastObjectToTransferObj(output);
                return output;
            }
            else { return null; }
        }
       
        public static bool WriteChannel(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            int bytesWritten = 0;
            bool returnValue = WtsApi32.WTSVirtualChannelWrite(mHandle, data, data.Length, ref bytesWritten);
            if(bytesWritten == data.Length)
            {
                Console.WriteLine($"Written string of {message} as a total of {data.Length.ToString()} bytes.");
            }
            return returnValue;
        }
        public static bool WriteChannel(TransferObj transObj)
        {
            PackerUnpacker utility = new PackerUnpacker();
            byte[] data = utility.PackObjectToByteArray(transObj);
            int bytesWritten = 0;
            bool returnValue = WtsApi32.WTSVirtualChannelWrite(mHandle, data, data.Length, ref bytesWritten);
            if (bytesWritten == data.Length)
            {
                Console.WriteLine($"TransferObj with name: {transObj.Name} has been sent as total of {data.Length.ToString()} bytes.");
            }
            return returnValue;
        }
        public static bool CloseChannel()
        {
            return WtsApi32.WTSVirtualChannelClose(mHandle);
        }
    }
}
