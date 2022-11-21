using System.Net;
using System.Net.Sockets;

namespace App;

internal class Server
{
    #region Properties

    private readonly TcpListener _server;

    private readonly List<ServerWorker> _workers = new List<ServerWorker>();

    public bool Initialized = false;

    public static IPAddress ServerAddress = default!;

    #endregion Properties

    #region Events

    private void ServerWorker_MessageReceived(object sender, MessageEventArgs e) => Broadcast((sender as ServerWorker)!, e.Message, e.SystemMessage);

    private void ServerWorker_Disconnected(object sender, EventArgs e) => RemoveServerWorker((sender as ServerWorker)!);

    #endregion Events

    public Server(int port)
    {
        Console.Write("Digite o IP para hostear (deixe em branco para localhost): ");
        string hostIp = Console.ReadLine()!;

        Console.WriteLine("Criando sala...");

        ServerAddress = Helper.GetIpAddress(hostIp);

        _server = new TcpListener(ServerAddress, port);
        _server.Start();

        Initialized = true;

        Console.WriteLine("Sala criada com sucesso, aguardando conexões");
    }

    public void Start() => new Thread(WaitConnections).Start();

    public void WaitConnections()
    {
        while (true)
        {
            TcpClient socket = _server.AcceptTcpClient();
            ServerWorker worker = new ServerWorker(socket);
            AddServerWorker(worker);
            worker.Start();
        }
    }

    private void Broadcast(ServerWorker fromWorker, string message, bool systemMessage)
    {
        lock (this)
        {
            message = systemMessage ? $"SYSTEM: {message}\r\n" : $"{fromWorker.Username}: {message}\r\n";

            for (int i = 0; i < _workers.Count; i++)
            {
                ServerWorker currentWorker = _workers[i];

                if (!currentWorker.Equals(fromWorker))
                {
                    try
                    {
                        currentWorker.Send(message);
                    }
                    catch
                    {
                        currentWorker.Close();
                        _workers.RemoveAt(i--);
                    }
                }
            }
        }
    }

    private void AddServerWorker(ServerWorker worker)
    {
        lock (this)
        {
            _workers.Add(worker);
            worker.Disconnected += ServerWorker_Disconnected!;
            worker.MessageReceived += ServerWorker_MessageReceived!;
            worker.Send("Digite seu username: ");
        }
    }

    private void RemoveServerWorker(ServerWorker worker)
    {
        lock (this)
        {
            worker.Disconnected -= ServerWorker_Disconnected!;
            worker.MessageReceived -= ServerWorker_MessageReceived!;
            worker.Close();
            _workers.Remove(worker);
            Broadcast(worker, $"{worker.Username} se desconectou", true);
        }
    }
}
