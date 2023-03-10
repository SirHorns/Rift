using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Rift.Player;

namespace Rift;

public class LogReader
{
    private readonly string _logsDirectory;
    private bool _loggingLock;
    private readonly bool _readLog;
    private readonly string _matchId;
    private Server _owner;

    public LogReader(string matchId, string logsDirectory, bool readLog, Server owner)
    {
        _matchId = matchId;
        _logsDirectory = logsDirectory;
        _loggingLock = true;
        _readLog = readLog;
        _owner = owner;
    }

    public async Task Read()
    {
        if (!_readLog)
        {
            return;
        }
        
        await Task.Delay(2000);
        var file = _logsDirectory + $"\\match{_matchId}.log";

        await using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.Default);
        while (_loggingLock)
        {
            var line = await sr.ReadLineAsync();
            if (string.IsNullOrEmpty(line))
            {
                await Task.Delay(1000);
            }
            else
            {
                await ParseLine(line);
            }
        }
    }
    
    private Task ParseLine(string line)
    {
        if (line.Contains("MatchStartConditions:"))
        {
            string pattern = @"(\d*(/)\d*)";
            var playerCount = Regex.Match(line, pattern);
            Console.WriteLine($"Players: {playerCount}");
        }
        else if (line.Contains("Blob from client:"))
        {
            var pJson = line.Split("Received Blob from client: ")[1];
            pJson = pJson.Split("!json")[0];
            var pJObject = JObject.Parse(pJson);
            var blob = pJObject.ToObject<PlayerBlob>();

            if (blob == null)
            {
                Console.WriteLine("Bad Player Blob.");
                return Task.CompletedTask;
            }

            _owner.ServerData.AddPlayer(blob);
        }

        return Task.CompletedTask;
    }

    public void Stop()
    {
        _loggingLock = false;
    }
}