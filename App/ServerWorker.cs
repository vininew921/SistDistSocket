using System.Net.Sockets;
using System.Text;

namespace App;

internal delegate void MessageEventHandler(object sender, MessageEventArgs e);

internal class ServerWorker
{
    public event MessageEventHandler MessageReceived = default!;

    public event EventHandler Disconnected = default!;

    public string? Username { get; set; }

    private readonly TcpClient _socket;
    private readonly Stream _socketStream;

    public ServerWorker(TcpClient socket)
    {
        _socket = socket;
        _socketStream = socket.GetStream();
    }

    public void Start() => new Thread(Run).Start();

    private void Run()
    {
        byte[] buffer = new byte[Helper.DEFAULT_BUFFER];
        try
        {
            while (true)
            {
                int receivedBytes = _socketStream.Read(buffer, 0, buffer.Length);
                if (receivedBytes < 1)
                {
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                message = message.Trim();

                if (!string.IsNullOrEmpty(message))
                {
                    if (Username == null)
                    {
                        Username = message;
                        string welcomeMessage = $"{Username} entrou!";
                        MessageReceived.Invoke(this, new MessageEventArgs(welcomeMessage, true));
                    }
                    else
                    {
                        MessageReceived.Invoke(this, new MessageEventArgs(message));
                    }
                }
            }
        }
        catch { }

        Disconnected.Invoke(this, EventArgs.Empty);
    }

    public void Close() => _socket.Close();

    public void Send(string message)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        _socketStream.Write(buffer, 0, buffer.Length);
    }
}