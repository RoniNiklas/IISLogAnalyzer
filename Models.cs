using System.Net;

namespace ServerLogAnalyzer;

public record struct LoggedDate(DateOnly Date, IEnumerable<LogRow> Logs);

/// <summary>
/// 
/// Looks like: 2023-12-06 00:00:04 <SERVERIP> GET /sv-FI/sidkarta opennodes=31150%2C31286%2C31283%2C31537%2C31364%2C31301 443 - <CLIENTIP> Mozilla/5.0+(compatible;+SemrushBot/7~bl;++http://www.semrush.com/bot.html) - 404 0 2 109
/// No idea what the opennodes thing after url is. Queryparams???
/// </summary>
/// <param name="RequestTimeTaken"></param>
public readonly record struct LogRow(DateTime TimeInitialized, string Method, string RelativeUrl, string ClientIP, string ClientDevice, HttpStatusCode ResponseCode, TimeSpan RequestTimeTaken)
{
    public readonly string AppName => RelativeUrl.Contains('/')
            ? RelativeUrl.Split('/')[1]
            : RelativeUrl;

    public static LogRow FromLogLine(string line, int index)
    {
        try
        {
            var lineParts = line.Split(' ');

            var requestUrl = lineParts[4].Trim().ToLower();
            if (requestUrl.Length > 1 && requestUrl[^1] == '/')
            {
                requestUrl = requestUrl[..^1];
            }

            return new LogRow(
                TimeInitialized: DateTime.Parse(lineParts[0] + " " + lineParts[1]),
                Method: string.Intern(lineParts[3]),
                RelativeUrl: string.Intern(requestUrl),
                ClientIP: string.Intern(lineParts[^7]),
                ClientDevice: string.Intern(lineParts[^6]),
                ResponseCode: (HttpStatusCode)int.Parse(lineParts[^4]),
                RequestTimeTaken: TimeSpan.FromMilliseconds(int.Parse(lineParts[^1])));
        }
        catch (Exception)
        {
            Console.Error.WriteLine($"Failed at reading line: {index}");
            throw;
        }
    }

    public static LogRow FromLogLineNew(string line, int index)
    {
        try
        {
            var lineParts = line.Split(' ');

            var requestUrl = lineParts[4].Trim().ToLower();
            if (requestUrl.Length > 1 && requestUrl[^1] == '/')
            {
                requestUrl = requestUrl[..^1];
            }

            return new LogRow(
                TimeInitialized: DateTime.Parse(lineParts[0] + " " + lineParts[1]),
                Method: string.Intern(lineParts[3]),
                RelativeUrl: string.Intern(requestUrl),
                ClientIP: string.Intern(lineParts[^7]),
                ClientDevice: string.Intern(lineParts[^6]),
                ResponseCode: (HttpStatusCode)int.Parse(lineParts[^4]),
                RequestTimeTaken: TimeSpan.FromMilliseconds(int.Parse(lineParts[^1])));
        }
        catch (Exception)
        {
            Console.Error.WriteLine($"Failed at reading line: {index}");
            throw;
        }
    }
}