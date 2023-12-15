// See https://aka.ms/new-console-template for more information
using ServerLogAnalyzer;
using System.Diagnostics;

Console.WriteLine("Give complete path to directory or leave empty to use the current directory");
var dir = Console.ReadLine();
if (string.IsNullOrWhiteSpace(dir))
{
    dir = "C:\\Users\\E1007795\\Desktop\\isov2logs"; //AppDomain.CurrentDomain.BaseDirectory;
}

dir = dir.Trim();

if (dir.Last() != '\\')
{
    dir += "\\"; // add trailing slash
}

var loggedDates = LogReader.ReadAllLogsFromDirectoryByDate(dir);

var vdCSVName = CSVWriter.WriteViewDocumentRequestTimes(loggedDates, dir);
Console.WriteLine("Press Y to open the ViewDocument CSV file");
Console.WriteLine("Press N to close the program");

var key = Console.ReadKey();
if (key.Key == ConsoleKey.Y)
{
    new Process()
    {
        StartInfo = new ProcessStartInfo { FileName = dir + vdCSVName, UseShellExecute = true }
    }.Start();
}