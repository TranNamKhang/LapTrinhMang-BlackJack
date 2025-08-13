// Sample Client using TCP for Blackjack game
using System;
using System.Net.Sockets;
using System.Text;

public class Client
{
    private TcpClient client;
    private NetworkStream stream;

    public void Connect(string ip, int port)
    {
        client = new TcpClient(ip, port);
        stream = client.GetStream();
    }

    public void SendMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    public string ReceiveMessage()
    {
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }
}