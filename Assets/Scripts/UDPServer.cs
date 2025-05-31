using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    public int port = 8888; // The port to listen on

    private UdpClient udpServer;

    void Start()
    {
        try
        {
            udpServer = new UdpClient(port);
            Debug.Log("UDP server started on port " + port);

            ListenForData();
        }
        catch (Exception e)
        {
            Debug.LogError("Error starting UDP server: " + e.Message);
        }
    }

    private async void ListenForData()
    {
        try
        {
            while (true)
            {
                UdpReceiveResult result = await udpServer.ReceiveAsync();
                byte[] data = result.Buffer;
                string receivedText = Encoding.UTF8.GetString(data);
                Debug.Log("Received: " + receivedText);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving UDP data: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        if (udpServer != null)
        {
            udpServer.Close();
        }
    }
}
