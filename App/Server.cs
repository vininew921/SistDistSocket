using System.Net;
using System.Net.Sockets;

namespace App;

internal class Server
{
    #region Properties

    private TcpListener _server = new TcpListener(IPAddress.Any, 3393);

    private readonly List<ServerWorker> _workers = new List<ServerWorker>();

    public bool Initialized = false;

    #endregion Properties

    #region Events

    private void ServerWorker_MessageReceived(object sender, MessageEventArgs e) => Broadcast((sender as ServerWorker)!, e.Message, e.SystemMessage);

    private void ServerWorker_Disconnected(object sender, EventArgs e) => RemoveServerWorker((sender as ServerWorker)!);

    #endregion Events

    public void Initialize(int port)
    {
        Console.WriteLine("Criando sala...");

        _server = new TcpListener(IPAddress.Any, port);
        _server.Start();

        Initialized = true;

        Console.WriteLine("Sala criada com sucesso, aguardando conexões");
    }

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