﻿using System;
using System.Runtime.InteropServices;

namespace Bizz_RDPVirtualChannelMessengerServer
{
    public class WtsApi32
    {
        [DllImport("Wtsapi32.dll")]
        public static extern IntPtr WTSVirtualChannelOpen(IntPtr server, int sessionId, [MarshalAs(UnmanagedType.LPStr)] string virtualName);

        [DllImport("Wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSVirtualChannelWrite(IntPtr channelHandle, byte[] data, int length, ref int bytesWritten);

        [DllImport("Wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSVirtualChannelRead(IntPtr channelHandle, int TimeOut, byte[] data, int length, ref int bytesReaded);

        [DllImport("Wtsapi32.dll")]
        public static extern bool WTSVirtualChannelClose(IntPtr channelHandle);
    }
}
