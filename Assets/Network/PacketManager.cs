using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct TouchPacket
{
    [MarshalAs(UnmanagedType.I4)]
    public int typeOfService;
    [MarshalAs(UnmanagedType.I4)]
    public int displayId;
    [MarshalAs(UnmanagedType.I4)]
    public int payloadLength;
    [MarshalAs(UnmanagedType.I4)]
    public int x;
    [MarshalAs(UnmanagedType.I4)]
    public int y;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct DirectionPacket
{
    [MarshalAs(UnmanagedType.I4)]
    public int typeOfService;
    [MarshalAs(UnmanagedType.I4)]
    public int displayId;
    [MarshalAs(UnmanagedType.I4)]
    public int payloadLength;
    [MarshalAs(UnmanagedType.I4)]
    public int direction;
}
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct TestPacket
{
    [MarshalAs(UnmanagedType.I4)]
    public int typeOfService;
    [MarshalAs(UnmanagedType.I4)]
    public int displayId;
    [MarshalAs(UnmanagedType.I4)]
    public int payloadLength;
    [MarshalAs(UnmanagedType.I4)]
    public int direction;
}

public class PacketManager
{
    // Packet to send
    private TouchPacket touchPacket = new TouchPacket();
    private DirectionPacket directionPacket = new DirectionPacket();
    private TestPacket testPacket = new TestPacket();

    // Display Id
    private int displayId;

    public PacketManager(int Id)
    {
        displayId = Id;
    }

    public byte[] GetTouchPacket(int x, int y)
    {
        touchPacket.typeOfService = 0;
        touchPacket.displayId = displayId;
        touchPacket.payloadLength = 8;
        touchPacket.x = x;
        touchPacket.y = y;

        return Serialize<TouchPacket>(touchPacket);
    }

    public byte[] GetDirectionPacket(int direction)
    {
        directionPacket.typeOfService = 1;
        directionPacket.displayId = displayId;
        directionPacket.payloadLength = 4;
        directionPacket.direction = direction;

        return Serialize<DirectionPacket>(directionPacket);
    }
    public byte[] GetTestPacket(int test)
    {
        testPacket.typeOfService = 1;
        testPacket.displayId = displayId;
        testPacket.payloadLength = 4;
        testPacket.direction = test;

        return Serialize<TestPacket>(testPacket);
    }

    // Calling this method will return a byte array with the contents
    // of the struct ready to be sent via the tcp socket.
    public static byte[] Serialize<T>(T packet)
    {
        // allocate a byte array for the struct data
        var buffer = new byte[Marshal.SizeOf(typeof(T))];

        // Allocate a GCHandle and get the array pointer
        var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        var pBuffer = gch.AddrOfPinnedObject();

        // copy data from struct to array and unpin the gc pointer
        Marshal.StructureToPtr(packet, pBuffer, false);
        gch.Free();

        return buffer;
    }
}