using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class Sender_UDP : MonoBehaviour
{

    public string destinationIP = "192.168.137.221";
    private static int sendDataPort = 9718;

    void Start() {
        DontDestroyOnLoad(this);
    }

    public void ChangeDestinationIP(string _ip) {
        destinationIP = _ip;
    }

    public void SendUdp(byte[] data) {
        using (UdpClient c = new UdpClient(9056))
            c.Send(data, data.Length, destinationIP, sendDataPort);
    }
}