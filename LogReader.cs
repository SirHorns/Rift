using System.Text;

namespace Rift;

public class LogReader
{
    private readonly string _logsDirectory;
    private bool Logging;

    public LogReader(string logsDirectory)
    {
        _logsDirectory = logsDirectory;
        Logging = true;
    }

    public async Task Read(string matchId)
    {
        await Task.Delay(2000);
        var file = _logsDirectory + $"\\match{matchId}.log";
        
        using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var sr = new StreamReader(fs, Encoding.Default)) {

            while (Logging)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    await Task.Delay(1000);
                }
                else
                {
                    if (line.Contains("MatchStartConditions"))
                    {
                        Console.WriteLine(line);
                    }
                    else if (line.Contains("Received Blob from client"))
                    {
                        Console.WriteLine(line);
                    }
                }
            }
        }
    }

    public void StopLogging()
    {
        Logging = false;
    }
}