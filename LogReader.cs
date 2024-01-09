using System.Diagnostics;

namespace ServerLogAnalyzer;
public class LogReader
{
    private const int MIN_LOG_ROW_COUNT = 20_000; // Assumed minimum number of log rows per file
    public static IEnumerable<LoggedDate> ReadAllLogsFromDirectoryByDate(string dir, bool useNewWay = true)
    {
        var start = Stopwatch.GetTimestamp();
        var logFiles = Directory.GetFiles(dir, "*.log");
        var loggedDates = new LoggedDate[logFiles.Length];

        Parallel.ForEach(logFiles.Select((x, fileIndex) => (x, fileIndex)), (tuple) =>
        {
            var (logFilePath, fileIndex) = tuple;
            Console.WriteLine($"Reading file {fileIndex + 1} of {logFiles.Length}: {logFilePath}");

            var loggedRows = new List<LogRow>(MIN_LOG_ROW_COUNT);
            using var reader = new StreamReader(logFilePath);
            var index = 0;
            var line = "";

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith('#')) // metadata rows
                {
                    continue;
                }

                if (line.Contains("sidkarta")) // common bot url
                {
                    continue;
                }

                index++;
                var logRow = useNewWay
                    ? LogRow.FromLogLineNew(line, index)
                    : LogRow.FromLogLine(line, index);
                loggedRows.Add(logRow);
            }
            var loggedDate = new LoggedDate(DateOnly.FromDateTime(loggedRows.First().TimeInitialized), loggedRows);

            loggedDates[fileIndex] = loggedDate;
        });

        var end = Stopwatch.GetTimestamp();
        Console.WriteLine($"Reading all logs took {(end - start) / (double)Stopwatch.Frequency} seconds");
        return loggedDates;
    }
}
