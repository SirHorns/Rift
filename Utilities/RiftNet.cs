using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Rift.Utilities;

public static class RiftNet
{

    public static async Task<string> GetOpenPort(int startingPort, int serverCap)
    {
        await Task.Delay(2000);
        int portStartIndex = startingPort;
        int portEndIndex = startingPort + serverCap;
        
        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] activeUdpListeners = properties.GetActiveUdpListeners();
 
        List<int> usedPorts = activeUdpListeners.Select(p => p.Port).ToList<int>();
        int unusedPort = 0;

        for (int port = portStartIndex; port < portEndIndex; port++)
        {
            if (!usedPorts.Contains(port))
            {
                unusedPort = port;
                break;
            }
        }
        
        Console.WriteLine($"Open port found: {unusedPort.ToString()}");
        return unusedPort.ToString();
    }
    
    public static bool ValidatePort(string port)
    {
        int portValue;
        
        if (!int.TryParse(port, out portValue))
        {
            Console.WriteLine($"{portValue} is not a valid port.");
            return false;
        }
        
        if (portValue > 65536)
        {
            Console.WriteLine("Provided port can not be over 65535");
            return false;
        }
        
        if (!CheckPortUsage(0, 65536))
        {
            Console.WriteLine($"{portValue} is already in use.");
            return false;
        }

        return true;
    }

    private static bool CheckPortUsage(int startingPort, int endPort)
    {
        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] activeUdpListeners = properties.GetActiveUdpListeners();

        List<int> usedPorts = activeUdpListeners.Select(p => p.Port).ToList<int>();
        int unusedPort = 0;

        for (int port = startingPort; port < endPort; port++)
        {
            if (!usedPorts.Contains(port))
            {
                return true;
            }
        }

        return false;
    }


    public static string GetExternalIpAddress()
    {
        var externalIpTask = QueryExternalIpAddress();
        QueryExternalIpAddress().Wait();
        var externalIpString = externalIpTask.Result ?? IPAddress.Loopback;
        return externalIpString.ToString();
    }
    
    private static async Task<IPAddress?> QueryExternalIpAddress()
    {
        var externalIpString = (await new HttpClient().GetStringAsync("http://ipv4.icanhazip.com"))
            .Replace("\\r\\n", "").Replace("\\n", "").Trim();
        if(!IPAddress.TryParse(externalIpString, out var ipAddress)) return null;
        return ipAddress;
    }
    
    public static string GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "[No network adapters with an IPv4 address in the system.]";
    }
}