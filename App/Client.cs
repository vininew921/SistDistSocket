using System.Net;
using System.Net.Sockets;
using System.Text;

namespace App;

internal class Client
{
    public bool Initialized = false;

    private readonly TcpClient _socket;
    private readonly Stream _socketStream;

    public Client(int port)
    {
        Console.WriteLine("IP: ");
        string ip = Console.ReadLine()!;

        IPAddress ipAddress = Helper.GetIpAddress(ip);
        IPEndPoint endpoint = new IPEndPoint(ipAddress, port);

        try
        {
            _socket = new TcpClient();
            _socket.Connect(endpoint);
            _socketStream = _socket.GetStream();
        }
        catch
        {
            Console.WriteLine("Conexão falhou");
        }
    }

    public void Start()
    {
        new Thread(FetchMessages).Start();
        ReadInput();
    }

    public void FetchMessages()
    {
        try
        {
            while (true)
            {
                byte[] buffer = new byte[Helper.DEFAULT_BUFFER];
                int receivedBytes = _socketStream.Read(buffer, 0, buffer.Length);

                if (receivedBytes < 1)
                {
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                message = message.Trim();

                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine(message);
                }
            }
        }
        catch { }

        _socket.Close();
    }

    public void ReadInput()
    {
        while (_socket.Connected)
        {
            string messageToSend = Console.ReadLine()!;
            messageToSend = messageToSend.Trim();

            if (!string.IsNullOrEmpty(messageToSend))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(messageToSend);
                _socketStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}