﻿using System;
using System.Runtime.InteropServices;


namespace Bizz_RDPVirtualChannelMessengerClient
{
    /// <summary>
    /// This is the specification of the dll required for virtual channel communication. 
    /// VIRTUALCHANNELENTRY is the entry point which should in turn call VirtualChannelInit function
    /// Huge assistance with this came from: https://www.codeproject.com/Articles/16374/How-to-Write-a-Terminal-Services-Add-in-in-Pure-C
    /// </summary>
    public class WtsApi32
    {
        // Event Delegates 
        public delegate ChannelReturnCodes VirtualChannelInitDelegate
            (ref IntPtr initHandle,
            ChannelDef[] channels, 
            int channelCount, 
            int versionRequested,
            [MarshalAs(UnmanagedType.FunctionPtr)] ChannelInitEventDelegate channelInitEventProc);

        public delegate ChannelReturnCodes VirtualChannelOpenDelegate
            (IntPtr initHandle, 
            ref int openHandle,
            [MarshalAs(UnmanagedType.LPStr)] string channelName,
            [MarshalAs(UnmanagedType.FunctionPtr)] ChannelOpenEventDelegate channelOpenEventProc);

        public delegate ChannelReturnCodes VirtualChannelWriteDelegate
            (int openHandle, 
            byte[] data, 
            uint dataLength, 
            IntPtr userData);

        public delegate ChannelReturnCodes VirtualChannelCloseDelegate
            (int openHandle);

        public delegate void ChannelInitEventDelegate
            (IntPtr initHandle,
            ChannelEvents Event,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data, 
            int dataLength);

        public delegate void ChannelOpenEventDelegate
            (int openHandle,
            ChannelEvents Event,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data,
            int dataLength, 
            uint totalLength, 
            ChannelFlags dataFlags);

        // Channel Entry points structure required for Virtual Channel communication.
        [StructLayout(LayoutKind.Sequential)]
        public struct ChannelEntryPoints
        {
            public int Size;
            public int ProtocolVersion;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public VirtualChannelInitDelegate VirtualChannelInit;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public VirtualChannelOpenDelegate VirtualChannelOpen;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public VirtualChannelCloseDelegate VirtualChannelClose;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public VirtualChannelWriteDelegate VirtualChannelWrite;
        }

        // Channel definition
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct ChannelDef
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string name;
            public ChannelOptions options;
        }

        // Channel Events
        // https://docs.microsoft.com/en-us/windows/desktop/api/cchannel/nc-cchannel-channel_init_event_fn
        public enum ChannelEvents
        {
            Initialized = 0,
            Connected = 1,
            V1Connected = 2,
            Disconnected = 3,
            Terminated = 4,
            DataRecived = 10,
            WriteComplete = 11,
            WriteCanceled = 12
        }

        // Channel Flags
        [Flags]
        public enum ChannelFlags
        {
            First = 0x01,
            Last = 0x02,
            Only = First | Last,
            Middle = 0,
            Fail = 0x100,
            ShowProtocol = 0x10,
            Suspend = 0x20,
            Resume = 0x40
        }
        // Further flags - this time options.
        [Flags]
        public enum ChannelOptions : uint
        {
            Initialized = 0x80000000,
            EncryptRDP = 0x40000000,
            EncryptSC = 0x20000000,
            EncryptCS = 0x10000000,
            PriorityHigh = 0x08000000,
            PriorityMedium = 0x04000000,
            PriorityLow = 0x02000000,
            CompressRDP = 0x00800000,
            Compress = 0x00400000,
            ShowProtocol = 0x00200000
        }

        // Channel return codes
        public enum ChannelReturnCodes
        {
            Ok = 0,
            AlreadyInitialized = 1,
            NotInitialized = 2,
            AlreadyConnected = 3,
            NotConnected = 4,
            TooManyChanels = 5,
            BadChannel = 6,
            BadChannelHandle = 7,
            NoBuffer = 8,
            BadInitHandle = 9,
            NotOpen = 10,
            BadProc = 11,
            NoMemory = 12,
            UnknownChannelName = 13,
            AlreadyOpen = 14,
            NotInVirtualchannelEntry = 15,
            NullData = 16,
            ZeroLength = 17
        }
    }
}
