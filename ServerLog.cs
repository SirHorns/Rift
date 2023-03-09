using System.Text;
using System.Text.RegularExpressions;

namespace Rift;

public static class ServerLog
{
    private static string[] Lines;
    public static void Read()
    {
        using var fs = new FileStream("F:\\Spellbreak_Server\\g3\\Saved\\Logs\\match7777.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using (StreamReader sr = new StreamReader(fs, Encoding.Default))
        {
            string log = sr.ReadToEnd();
            Lines = log.Split("\n");
            ParseLine();
            Console.WriteLine(log);
        }
    }

    private static void ParseLine()
    {
        int lineCount = -1;
        foreach (var line in Lines)
        {
            if (!useRegex(line))
            {
                lineCount++;
            }
            else
            {
                break;
            }
        }
    }
    
    public static bool useRegex(String input)
    {
        Regex regex = new Regex("\\[[^\\]]*]", RegexOptions.IgnoreCase);
        return regex.IsMatch(input);
    }
}