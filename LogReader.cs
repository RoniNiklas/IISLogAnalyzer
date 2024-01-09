using System.Diagnostics;

namespace ServerLogAnalyzer;
public class LogReader
{
    private const int MIN_LOG_ROW_COUNT = 20_000; // Assumed minimum number of log rows per file
    public static IEnumerable<LoggedDate> ReadLogsFromDirectory(string dir, int maxFileCount, bool useNewWay = true)
    {
        var start = Stopwatch.GetTimestamp();
        var dirInfo = new DirectoryInfo(dir);
        var logFiles = dirInfo
            .GetFiles("*.log")
            .OrderByDescending(e => e.LastWriteTime)
            .Take(maxFileCount)
            .ToArray();
        var loggedDates = new LoggedDate[logFiles.Length];

        Parallel.ForEach(logFiles.Select((x, fileIndex) => (x, fileIndex)), (tuple) =>
        {
            var (logFile, fileIndex) = tuple;
            Console.WriteLine($"Starting to read file {fileIndex + 1} of {logFiles.Length}: {logFile.FullName}");

            var loggedRows = new List<LogRow>(MIN_LOG_ROW_COUNT);
            using var reader = new StreamReader(logFile.FullName);
            var index = 0;
            var line = "";

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith('#')) // metadata rows
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

            Console.WriteLine($"Done reading file {fileIndex + 1} of {logFiles.Length}: {logFile.FullName}");
        });

        var end = Stopwatch.GetTimestamp();
        Console.WriteLine($"Reading all logs took {(end - start) / (double)Stopwatch.Frequency} seconds");
        return loggedDates;
    }
}
