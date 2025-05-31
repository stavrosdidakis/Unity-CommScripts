using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
    [Header("TCP Client Settings")]
    public string serverIP = "127.0.0.1"; // Editable in Inspector
    public int serverPort = 8888;         // Editable in Inspector

    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[4096];

    void Start()
    {
        StartClient();
    }

    void StartClient()
    {
        try
        {
            client = new TcpClient();
            client.BeginConnect(serverIP, serverPort, ClientConnected, null);
            Debug.Log($"Attempting to connect to {serverIP}:{serverPort}...");
        }
        catch (Exception e)
        {
            Debug.LogError("Error starting client: " + e.Message);
        }
    }

    void ClientConnected(IAsyncResult result)
    {
        try
        {
            client.EndConnect(result);
            Debug.Log("Connected to server.");

            stream = client.GetStream();
            stream.BeginRead(buffer, 0, buffer.Length, DataReceived, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to server: " + e.Message);
        }
    }

    void Update()
    {
        // Send a message whenever any key is pressed
        if (Input.anyKeyDown)
        {
            SendData("Hello from client! Key pressed: " + Input.inputString);
        }
    }

    void DataReceived(IAsyncResult result)
    {
        int bytesRead = stream.EndRead(result);
        if (bytesRead <= 0)
        {
            Debug.Log("Disconnected from server.");
            return;
        }

        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Debug.Log("Received from server: " + message);

        stream.BeginRead(buffer, 0, buffer.Length, DataReceived, null);
    }

    public void SendData(string message)
    {
        if (stream == null)
        {
            Debug.LogWarning("Cannot send data: No connection established.");
            return;
        }

        byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
        Debug.Log("Sent: " + message);
    }

    void OnApplicationQuit()
    {
        if (client != null)
        {
            client.Close();
        }
    }
}
