using System.Diagnostics;

namespace Rift;

public class Server
{
    private readonly Process _process;
    public string MatchId { get; private set; }
    public string Args { get; }
    public string Port { get; }
    public bool IsRunning { get; private set; }

    public static event EventHandler? Started;
    public static event EventHandler? Stopped;
    public static event EventHandler? Exited;

    public LogReader LogReader { get; }

    public Server(Process process, string args, string port, string matchId = "", bool autoStart = false)
    {
        _process = process;
        _process.Exited += ProcessExit;
        _process.EnableRaisingEvents = true;
        
        Args = args;
        Port = port;

        IsRunning = false;
        MatchId = port;

        LogReader = new LogReader("F:\\Spellbreak_Server\\g3\\Saved\\Logs");

        if (autoStart)
        {
            Start();
        }
    }
    
    public bool Start()
    {
        Console.WriteLine($"Starting server {Port}");
        
        try
        {
            _process.Start();
            Task.Run(() => LogReader.Read(MatchId));
        }
        catch (Exception e)
        {
            Console.WriteLine($"Server {Port} was unable to start.");
            Console.WriteLine(e);
            return IsRunning = false;
        }
        
        Started?.Invoke(this, EventArgs.Empty);
        return IsRunning = true;
    }
    
    public bool Stop()
    {
        Console.WriteLine($"Stopping server {Port}");
        try
        {
            _process.Kill();
            LogReader.StopLogging();
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
}