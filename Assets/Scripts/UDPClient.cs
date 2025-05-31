using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    [Header("UDP Client Settings")]
    public string serverIP = "127.0.0.1"; // Editable in the Inspector
    public int serverPort = 8888;         // Editable in the Inspector

    private UdpClient client;
    private IPEndPoint serverEndPoint;

    void Start()
    {
        StartClient();
    }

    void StartClient()
    {
        try
        {
            client = new UdpClient();
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            Debug.Log($"UDP client configured to send to {serverIP}:{serverPort}");
        }
        catch (Exception e)
        {
            Debug.LogError("Error starting client: " + e.Message);
        }
    }

    void Update()
    {
        // Send a message whenever any key is pressed
        if (Input.anyKeyDown)
        {
            SendData("Hello from the client! Key pressed: " + Input.inputString);
        }
    }

    void SendData(string message)
    {
        try
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            client.Send(data, data.Length, serverEndPoint);
            Debug.Log("Sent: " + message);
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending data: " + e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        client.Close();
    }
}
