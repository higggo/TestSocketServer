using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Character : MonoBehaviour
{
    public TMP_InputField register_username; //drag and drop element

    public void getValue()
    {
        string username = register_username.text;
    }
    public void SendTestPacket()
    {
        string text = register_username.text;
        int id = int.Parse(text);
        Debug.Log("id : " + id + "text : " + text);
        PacketManager pm = new PacketManager(1234);
        byte[] dirPacket = pm.GetTestPacket(1);
        SocketClient.Instance.SendMessage(dirPacket);
    }
}
