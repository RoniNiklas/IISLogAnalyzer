using System.Text;

namespace ServerLogAnalyzer;
public class CSVWriter
{
    private const char CSV_SEPARATOR = ';';
    private static string GetCSVRow(params object[] values)
    {
        var sb = new StringBuilder();
        foreach (var value in values)
        {
            sb.Append(value.ToString() + CSV_SEPARATOR);
        }
        return sb.ToString();
    }

    public static string WriteApplicationAccessCountAndTimeAveragePerRequest(IEnumerable<LoggedDate> loggedDates, string[] ignoredFileEndings, string directory)
    {
        Console.WriteLine("Writing request counts and average times per app");
        var sb = new StringBuilder();
        sb.AppendLine(GetCSVRow("Application", "Count", "AverageTimePerRequest"));

        var requestCountAndAverageTimeTakenPerApplication = loggedDates
            .SelectMany(x => x.Logs)
            .Where(e => e.RelativeUrl != null
                && !ignoredFileEndings.Any(ending => e.RelativeUrl.EndsWith(ending))
                && !e.RelativeUrl.Contains("favicon"))
            .GroupBy(logRow => logRow.AppName)
            .Select(appGroup => new
            {
                AppName = appGroup.Key,
                CountOfRequests = appGroup.Count(),
                AverageTimeTaken = appGroup.Average(logRow => logRow.RequestTimeTaken.TotalMilliseconds)
            })
            .OrderByDescending(x => x.CountOfRequests)
            .ToList();

        foreach (var appGroup in requestCountAndAverageTimeTakenPerApplication)
        {
            sb.AppendLine(GetCSVRow(appGroup.AppName, appGroup.CountOfRequests, (int)double.Round(appGroup.AverageTimeTaken)));
        }

        var fileName = "ApplicationAccessCountAndTimeAveragePerRequest.csv";
        File.WriteAllText(directory + fileName, sb.ToString());
        return fileName;

    }

    public static string WriteViewDocumentRequestTimesPerDate(IEnumerable<LoggedDate> loggedDates, string directory)
    {
        Console.WriteLine("Writing ViewDocument request times");
        var sb = new StringBuilder();
        sb.AppendLine(GetCSVRow("Date", "AverageTimeTaken"));
        var requestTimePerDatePerMethodPerURLInSiirtoRekisteri = loggedDates
            .GroupBy(loggedDate => loggedDate.Date)
            .Select(dateGroup => new
            {
                Date = dateGroup.Key,
                RequestTimePerMethodPerURL = dateGroup
                    .SelectMany(loggedDate => loggedDate.Logs)
                    .Where(logRow => logRow.AppName == "siirto"
                        && logRow.Method == "POST"
                        && logRow.RelativeUrl.Contains("viewdocument")
                        && !logRow.RelativeUrl.Contains("redirect")
                        && !logRow.RelativeUrl.Contains("delete")
                        && !logRow.RelativeUrl.Contains("returnurl"))
                    .Select(x => new
                    {
                        RelativeUrl = x.Method == "PUT" || x.Method == "GET"
                            ? x.RelativeUrl[..x.RelativeUrl.LastIndexOf('/')]
                            : x.RelativeUrl,
                        x.Method,
                        x.RequestTimeTaken
                    })
                    .GroupBy(logRow => logRow.RelativeUrl)
                    .Select(urlGroup => new
                    {
                        RequestUrl = urlGroup.Key,
                        RequestTimePerMethod = urlGroup
                            .GroupBy(logRow => logRow.Method)
                            .Select(methodGroup => new
                            {
                                Method = methodGroup.Key,
                                CountOfRequests = methodGroup.Count(),
                                AverageTimeTaken = methodGroup.Average(logRow => logRow.RequestTimeTaken.TotalMilliseconds)
                            })
                            .OrderBy(x => x.Method)
                    })
                    .OrderBy(x => x.RequestUrl)
            })
            .OrderBy(x => x.Date)
            .ToList();

        foreach (var dateGroup in requestTimePerDatePerMethodPerURLInSiirtoRekisteri)
        {
            foreach (var urlGroup in dateGroup.RequestTimePerMethodPerURL)
            {
                foreach (var methodGroup in urlGroup.RequestTimePerMethod)
                {
                    sb.AppendLine(GetCSVRow(dateGroup.Date.ToString("dd.MM.yyyy"), (int)double.Round(methodGroup.AverageTimeTaken)));
                }
            }
        }
        var fileName = "ViewDocumentRequestTimesPerDate.csv";
        File.WriteAllText(directory + fileName, sb.ToString());
        return fileName;
    }
}
