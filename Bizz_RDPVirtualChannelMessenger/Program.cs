using System;
using System.Text;
using Bizz_RDPVirtualChannelMessenger;

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
                        var ret = VirtualChannel.WriteChannel(Console.ReadLine());
                        Console.WriteLine($"Write to channel returned: {ret}.");
                        Console.WriteLine("Press enter to continue...");
                        Console.ReadLine();
                        DisplayChoice();
                        break;
                    }
                case ConsoleKey.D2:
                    {
                        Console.WriteLine(VirtualChannel.ReadChannel());
                        DisplayChoice();
                        break;
                    }
                case ConsoleKey.Spacebar:
                    {
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

        public static string ReadChannel()
        {
            return "Not yet implemented";
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
    }
}
