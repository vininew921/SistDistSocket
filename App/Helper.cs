using System.Net;
using System.Net.Sockets;

namespace App;

internal static class Helper
{
    public const int DEFAULT_BUFFER = 1_024;

    public static IPAddress GetIpAddress(string? address = null)
    {
        //Se nenhum address for passado, usar address local
        if (address == null || string.IsNullOrEmpty(address) || address == "localhost")
        {
            IPHostEntry host = Dns.GetHostEntry("localhost")!;

            return host.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
        }
        else
        {
            IPHostEntry? host = Dns.GetHostEntry(address);
            return host.AddressList.First();
        }
    }
}