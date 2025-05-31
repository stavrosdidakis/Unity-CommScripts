using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    [Header("TCP Server Settings")]
    public int port = 8888; // Editable in Inspector

    private TcpListener server;
    private List<TcpClient> clients = new List<TcpClient>();
    private List<NetworkStream> clientStreams = new List<NetworkStream>();
    private byte[] buffer = new byte[4096];

    void Start()
    {
        StartServer();
    }

    void StartServer()
    {
        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Debug.Log("Server started on port " + port + ". Waiting for clients...");

            server.BeginAcceptTcpClient(ClientConnected, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Error starting server: " + e.Message);
        }
    }

    void ClientConnected(IAsyncResult result)
    {
        TcpClient client = server.EndAcceptTcpClient(result);
        clients.Add(client);

        Debug.Log("Client connected: " + client.Client.RemoteEndPoint);

        NetworkStream stream = client.GetStream();
        clientStreams.Add(stream);

        stream.BeginRead(buffer, 0, buffer.Length, DataReceived, client);
        server.BeginAcceptTcpClient(ClientConnected, null);
    }

    void DataReceived(IAsyncResult result)
    {
        TcpClient client = (TcpClient)result.AsyncState;
        NetworkStream stream = client.GetStream();

        int bytesRead = stream.EndRead(result);
        if (bytesRead <= 0)
        {
            ClientDisconnected(client);
            return;
        }

        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Debug.Log("Received from " + client.Client.RemoteEndPoint + ": " + message);

        // Broadcast to all clients
        BroadcastMessage(message);

        stream.BeginRead(buffer, 0, buffer.Length, DataReceived, client);
    }

    void BroadcastMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        foreach (var stream in clientStreams)
        {
            stream.Write(data, 0, data.Length);
        }
    }

    void ClientDisconnected(TcpClient client)
    {
        Debug.Log("Client disconnected: " + client.Client.RemoteEndPoint);
        clientStreams.Remove(client.GetStream());
        clients.Remove(client);
    }

    void OnApplicationQuit()
    {
        if (server != null)
        {
            server.Stop();
        }

        foreach (var client in clients)
        {
            client.Close();
        }
    }
}
