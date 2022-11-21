namespace App;

internal static class Application
{
    private static AppType _appType = AppType.Undefined;
    private static readonly int _port = 11_000;

    public static void Initialize()
    {
        while (_appType == AppType.Undefined)
        {
            Console.WriteLine("Selecione uma das opções: \n1 - Criar sala\n2 - Entrar em uma sala\n3 - Encerrar\n");
            string? choice = Console.ReadLine();

            if (choice == null)
            {
                Console.WriteLine("Opção inválida");
                continue;
            }

            try
            {
                _appType = (AppType)int.Parse(choice);
            }
            catch
            {
                Console.WriteLine("Opção inválida");
            }
        }

        Console.Clear();
    }

    public static void Run()
    {
        switch (_appType)
        {
            case AppType.Server:
                RunAsServer();
                break;

            case AppType.Client:
                RunAsClient();
                break;

            case AppType.Shutdown:
                return;

            case AppType.Undefined:
            default:
                Console.WriteLine("Isso nao deveria ser possivel xdd");
                break;
        }
    }

    private static void RunAsServer()
    {
        Server server = new Server();
        server.Initialize(_port);
        if (server.Initialized)
        {
            server.WaitConnections();
        }
    }

    private static void RunAsClient()
    {
        Client client = new Client(_port);
        client.Start();
    }

    public static void Cleanup()
    {
    }
}