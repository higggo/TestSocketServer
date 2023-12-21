using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.XR;

public class SocketClient : MonoBehaviour
{
    private TcpClient socketConnection;
    private static SocketClient instance = null;
    private Thread clientReceiveThread;
    CallbackTouch callbackTouch;
    CallbackDirection callbackDirection;
    CallbackGaze callbackGaze;
    CallbackVoice callbackVoice;
    CallbackHandSkeleton callbackHandSkeleton;

    NetworkStream stream;
    public void StartClient()
    {
        ConnectToTcpServer();
    }

    public static SocketClient Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    private void Update()
    {
        //Debug.Log("conn : " + socketConnection.Connected);
    }
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();

            instance = this;
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("localhost", 50003);
            Debug.Log("ListenForData new conn true : " + socketConnection.Connected);
            while (true)
            {
                Debug.Log("ListenForData conn true : " + socketConnection.Connected);
                // Get a stream object for reading 				
                using (stream = socketConnection.GetStream())
                {                        
                    // Read incomming stream into byte array.
                    Debug.Log("ListenForData GetStream conn : " + socketConnection.Connected);
                    while (stream.CanRead)
                    {
                        Byte[] bytesTypeOfService = new Byte[4];

                        int lengthTypeOfService = stream.Read(bytesTypeOfService, 0, 4);
                        //int lengthDisplayId = stream.Read(bytesDisplayId, 0, 4);
                        //int lengthPayloadLength = stream.Read(bytesPayloadLength, 0, 4);

                        if (lengthTypeOfService <= 0)
                        {
                        }
                        else
                        {
                            // Reverse byte order, in case of big endian architecture
                            if (!BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(bytesTypeOfService);
                            }
                            Debug.Log("Execute lengthTypeOfService : " + lengthTypeOfService);
                        }

                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    /// Send message to server using socket connection.     
    public void SendMessage(Byte[] buffer)
    {

        if (socketConnection == null)
        {
            return;
        }
        try
        {

            Debug.Log("SendMessage1 conn : " + socketConnection.Connected);
            // Get a stream object for writing.             
            NetworkStream stream = socketConnection.GetStream();
            Debug.Log("SendMessage2 conn : " + socketConnection.Connected);
            if (stream.CanWrite)
            {
                Debug.Log("SendMessage3 conn : " + socketConnection.Connected);
                // Write byte array to socketConnection stream.                 
                stream.Write(buffer, 0, buffer.Length);
            }
            Debug.Log("SendMessage4 conn : " + socketConnection.Connected);
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void SetTouchCallback(CallbackTouch callback)
    {
        if (callbackTouch == null)
        {
            callbackTouch = callback;
        }
        else
        {
            callbackTouch += callback;
        }
    }

    public void SetDirectionCallback(CallbackDirection callback)
    {
        if (callbackDirection == null)
        {
            callbackDirection = callback;
        }
        else
        {
            callbackDirection += callback;
        }
    }

    public void SetGazeCallback(CallbackGaze callback)
    {
        if (callbackGaze == null)
        {
            callbackGaze = callback;
        }
        else
        {
            callbackGaze += callback;
        }
    }

    public void SetVoiceCallback(CallbackVoice callback)
    {
        if (callbackVoice == null)
        {
            callbackVoice = callback;
        }
        else
        {
            callbackVoice += callback;
        }
    }

    public void SetHandSkeletonCallback(CallbackHandSkeleton callback)
    {
        if (callbackHandSkeleton == null)
        {
            callbackHandSkeleton = callback;
        }
        else
        {
            callbackHandSkeleton += callback;
        }
    }

    // Handle incomming request
    private void HandleIncommingRequest(int typeOfService, int displayId, int payloadLength, byte[] bytes)
    {
        Debug.Log("=========================================");
        Debug.Log("Type of Service : " + typeOfService);
        Debug.Log("Display Id      : " + displayId);
        Debug.Log("Payload Length  : " + payloadLength);
        switch (typeOfService)
        {
            case 0:
                TouchHandler(displayId, payloadLength, bytes);
                break;
            case 1:
                DirectionHander(displayId, payloadLength, bytes);
                break;
            case 2:
                GazeHandler(displayId, payloadLength, bytes);
                break;
            case 3:
                VoiceHandler(displayId, payloadLength, bytes);
                break;
            case 4:
                BodySkeletonHandler(displayId, payloadLength, bytes);
                break;
            case 5:
                HandSkeletonHandler(displayId, payloadLength, bytes);
                break;
        }
    }

    // Handle Touch Signal
    private void TouchHandler(int displayId, int payloadLength, byte[] bytes)
    {
        Debug.Log("Execute Touch Handler");
        int x_axis = BitConverter.ToInt32(bytes, 0);
        int y_axis = BitConverter.ToInt32(bytes, 4);
        Debug.Log("X axis     : " + x_axis);
        Debug.Log("Y axis     : " + y_axis);
        if (callbackTouch != null)
        {
            callbackTouch(x_axis, y_axis);
        }
    }

    // Handle Direction Signal
    private void DirectionHander(int displayId, int payloadLength, byte[] bytes)
    {
        Debug.Log("Execute Direction Handler");
        int direction = BitConverter.ToInt32(bytes, 0);
        Debug.Log("Direction  : " + direction);


        //PacketManager pm = new PacketManager(displayId);
        //byte[] dirPacket = pm.GetDirectionPacket(direction);
        //SendMessage(dirPacket);

        if (callbackDirection != null)
        {
            callbackDirection(direction);
        }

    }

    // Handle Gaze Signal
    private void GazeHandler(int displayId, int payloadLength, byte[] bytes)
    {
        Debug.Log("Execute Gaze Handler");
        int x_axis = BitConverter.ToInt32(bytes, 0);
        int y_axis = BitConverter.ToInt32(bytes, 4);
        Debug.Log("X axis     : " + x_axis);
        Debug.Log("Y axis     : " + y_axis);
        if (callbackGaze != null)
        {
            callbackGaze(x_axis, y_axis);
        }
    }

    // Handle Voice Signal
    private void VoiceHandler(int displayId, int payloadLength, byte[] bytes)
    {
        Debug.Log("Execute Voice Handler");
        string str = Encoding.Default.GetString(bytes);
        Debug.Log("Text       : " + str);
        if (callbackVoice != null)
        {
            callbackVoice(str);
        }
    }

    // Handle Body Skeleton Signal
    private void BodySkeletonHandler(int displayId, int payloadLength, byte[] bytes)
    {
        Debug.Log("Execute Body Skeleton Handler");
        // TODO
    }

    // Handle Hand Skeleton Signal
    private void HandSkeletonHandler(int displayId, int payloadLength, byte[] bytes)
    {
        Debug.Log("Execute Hand Skeleton Handler");
        int x_axis = BitConverter.ToInt32(bytes, 0);
        int y_axis = BitConverter.ToInt32(bytes, 4);
        Debug.Log("X axis     : " + x_axis);
        Debug.Log("Y axis     : " + y_axis);
        if (callbackHandSkeleton != null)
        {
            callbackHandSkeleton(x_axis, y_axis);
        }
    }
}