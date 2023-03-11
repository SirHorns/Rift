using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Rift.Events;
using Rift.Player;

namespace Rift;

public class Server
{
    private readonly Process _process;
    public string MatchId { get; private set; }
    public string Args { get; }
    public string Port { get; }
    public bool IsRunning { get; private set; }
    public ServerData ServerData { get; }

    public static event EventHandler? Started;
    public static event EventHandler? Stopped;
    public static event EventHandler? Exited;
    public static event EventHandler? PlayerJoin;

    public LogReader LogReader { get; }

    public Server(Process process, string args, string port, string matchId, string logDirectory, bool readLogFile = false, bool autoStart = false)
    {
        _process = process;
        _process.Exited += ProcessExit;
        _process.EnableRaisingEvents = true;
        
        Args = args;
        Port = port;

        IsRunning = false;
        MatchId = matchId;
        
        ServerData = new ServerData();
        ServerData.PlayedAdded += OnPlayerAdded;

        LogReader = new LogReader(matchId, logDirectory, readLogFile, this);
        
        if (autoStart)
        {
            Start();
        }
    }

    private void OnPlayerAdded(object? sender,EventArgs? e)
    {
        if (e is not PlayerArg arg)
        {
            return;
        }
        
        PlayerJoin?.Invoke(this, new PlayerArg(arg.Player));
    }


    public bool Start()
    {
        Console.WriteLine($"{Port} -Starting");
        
        try
        {
            _process.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Server {Port} was unable to start.");
            Console.WriteLine(e);
            return IsRunning = false;
        }
        
        Task.Run(() => LogReader.Read());
        Started?.Invoke(this, EventArgs.Empty);
        return IsRunning = true;
    }
    
    public bool Stop()
    {
        Console.WriteLine($"Stopping server {Port}");
        try
        {
            _process.Kill();
            LogReader.Stop();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        
        Stopped?.Invoke(this, EventArgs.Empty);
        return true;
    }
    
    private void ProcessExit(object? sender, EventArgs e)
    {
        IsRunning = false;
        Exited?.Invoke(this, EventArgs.Empty);
    }

    public void OnPlayerJoin()
    {
        
    }
}