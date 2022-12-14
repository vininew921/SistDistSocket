using System.Net;
using System.Net.Sockets;
using System.Text;

namespace App;

internal class Client
{
    public bool Initialized = false;

    private readonly TcpClient _socket = default!;
    private readonly Stream _socketStream = default!;

    public Client(int port, bool serverAsClient)
    {
        IPAddress ipAddress;

        if (!serverAsClient)
        {
            Console.Write("IP: ");
            string ip = Console.ReadLine()!;
            ipAddress = Helper.GetIpAddress(ip);
        }
        else
        {
            ipAddress = Server.ServerAddress;
        }

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
                    RoomUser? user = RoomUser.GetFromMessage(message);

                    Console.ForegroundColor = user == null ? ConsoleColor.White : user.Color;
                    Console.WriteLine(message);
                    Console.ForegroundColor = ConsoleColor.White;
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
