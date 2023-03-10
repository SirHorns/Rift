using System.Diagnostics;
using Rift.Events;
using Rift.Utilities;

namespace Rift;

public class ServerManager
{
    public string ServerExecutable = "g3Server-Win64-Test.exe";
    public int ServerCap { get; set; }
    public int QueueCap { get; set; }
    public int StartingPort { get; set; }
    public  string ServerRootPath { get; set; }
    public string ExternalIpAddress { get; private set; }
    public string LocalIpAddress { get; private set; }
    
    public string ExecutableDirectory { get; }

    public static string LogsDirectory { get; private set; }
    public Dictionary<string, Server?> Servers { get; set; }
    public int ServerCount => Servers.Count;
    private Server[] QueuedMatches {get; set;}

    public event EventHandler? ServerStarted;
    public event EventHandler? ServerStopped;
    
    public ServerManager(string serverRootPath, string localIpAddress, string externalIpAddress, int serverCap = 5, int queueCap = 5, int startingPort = 7777)
    {
        ServerRootPath = serverRootPath;
        LogsDirectory = Path.GetFullPath(serverRootPath + "//g3//Saved//Logs") ;
        ExecutableDirectory = Path.GetFullPath(serverRootPath + "//g3//Binaries//Win64");

        var exe = Path.GetFullPath(ExecutableDirectory + "\\" + ServerExecutable);
        
        if (!File.Exists(exe))
        {
            throw new FileNotFoundException($"[{exe}] is not a valid path to {ServerExecutable}");
        }
        
        ServerCap = serverCap;
        QueueCap = queueCap;
        StartingPort = startingPort;
        Servers = new();
        QueuedMatches = new Server[QueueCap];
        LocalIpAddress = localIpAddress;
        ExternalIpAddress = externalIpAddress;
        
        Server.Started += OnServerStart;
        Server.Stopped += OnServerStop;
    }
    
    public bool CreateSever(
        out Server? server,
        string port = "",
        bool autoStart = false, 
        bool bypassServerCap = false)
    {
        server = null;
        
        if (ServerCount >= ServerCap && !bypassServerCap)
        {
            Console.WriteLine($"Server cap has been reached {ServerCount}/{ServerCap}\n" +
                              $"If you wish to add past the set cap you will need to manually create a server.");
            return false;
        }

        if (bypassServerCap)
        {
            if (!RiftNet.ValidatePort(port))
            {
                Console.WriteLine($"A valid port need to be given if you wish to bypass the server-cap.");
                return false;
            }
        }
        
        if (string.IsNullOrEmpty(port))
        {
            Console.WriteLine("Generating a new port.");
            port = RiftNet.GetOpenPort(StartingPort,ServerCap).Result;
        }
        else
        {
            if (!RiftNet.ValidatePort(port))
            {
                return false;
            }
        }

        if (Servers.ContainsKey(port))
        {
            Console.WriteLine("Server already exists on that port");
            return false;
        }
        
        Console.WriteLine("Creating new server session...");
        
        var args = $"?listen?name=Server -server port={port} -log log=match{port}.log";
        
        //Create new server process instance.
        
        var serverProcess = new Process();
        
        serverProcess.StartInfo.UseShellExecute = true;
        serverProcess.StartInfo.FileName = ServerExecutable;
        serverProcess.StartInfo.WorkingDirectory = ExecutableDirectory;
        serverProcess.StartInfo.Arguments = args;

        server = new Server(
            serverProcess,
            args,
            port,
            port,
            LogsDirectory,
            true,
            autoStart);
        
        Servers.Add(port, server);

        return true;
    }
    private void OnServerStop(object? sender, EventArgs e)
    {
        var server = ((ServerArg)e).Server;
        Console.WriteLine($"\n[Server-{server.Port} has stopped]");
        OnServerStopped(server);
    }
    private void OnServerStart(object? sender, EventArgs e)
    {
        var server = ((ServerArg)e).Server;
        Console.WriteLine($"[{ExternalIpAddress}:{server.Port} is now available]");
        OnServerStarted(server);
    }

    public bool StartServer(string port)
    {
        if (Servers.ContainsKey(port))
        {
            return Servers[port].Start();
        }

        Console.WriteLine($"There are no registered servers under {port}.");

        return false;
    }
    public bool StopServer(string port)
    {
        if (Servers.TryGetValue(port, out var server))
        {
            
            return server.Stop();
        }
        else
        {
            Console.WriteLine($"There are no registered servers under {port}.");
        }

        return false;
    }
    public bool StopServers()
    {
        Console.WriteLine("[KILLING ALL ACTIVE SEVERS]");
        try
        {
            foreach (var server in Servers.Values)
            {
                server.Stop();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        

        Servers.Clear();
        return true;
    }

    public string KillServers()
    {
        int processCount = 0;
        foreach (var process in Process.GetProcessesByName("g3Server-Win64-Test"))
        {
            process.Kill();
            processCount++;
        }
        
        Servers.Clear();

        return $"Killed {processCount} server process.";
    }
    
    public List<Server?> GetServers()
    {
        return Servers.Values.ToList();
    }
    
    public void OnServerStarted(Server server)
    {
        ServerStarted?.Invoke(this, new ServerArg(server));
    }
    public void OnServerStopped(Server server)
    {
        ServerStopped?.Invoke(this, new ServerArg(server));
    }
}