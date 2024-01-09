using System.Net;

namespace ServerLogAnalyzer;

public record struct LoggedDate(DateOnly Date, IEnumerable<LogRow> Logs);

/// <summary>
/// 
/// Looks like: 2023-12-06 00:00:04 193.166.21.152 GET /sv-FI/sidkarta opennodes=31150%2C31286%2C31283%2C31537%2C31364%2C31301 443 - 185.191.171.19 Mozilla/5.0+(compatible;+SemrushBot/7~bl;++http://www.semrush.com/bot.html) - 404 0 2 109
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
            // the url containing spaces in the query string is a problem
            var lineParts = line.Split(' ');

            // For some stupid reason there can be a space in the url so we can't use linesplitting to get it, so we use the magical numbers 80 and 443 (port numbers?) as the end of the url.
            var urlStart = line.IndexOf(lineParts[3]) + lineParts[3].Length;
            var hasPort443 = line.Contains(" 443 ");

            var urlEnd = hasPort443
                    ? line.IndexOf(" 443 ", urlStart)
                    : line.IndexOf(" 80 ", urlStart);
            var requestUrl = line[urlStart..urlEnd].Trim().ToLower();
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
            // the url containing spaces in the query string is a problem
            var lineParts = line.Split(' ');

            // For some stupid reason there can be a space in the url so we can't use linesplitting to get it, so we use the magical numbers 80 and 443 (port numbers?) as the end of the url.
            var urlStart = line.IndexOf(lineParts[3]) + lineParts[3].Length;
            var hasPort443 = line.Contains(" 443 ");

            var urlEnd = hasPort443
                    ? line.IndexOf(" 443 ", urlStart)
                    : line.IndexOf(" 80 ", urlStart);
            var requestUrl = line[urlStart..urlEnd].Trim().ToLower();
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