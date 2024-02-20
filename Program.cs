// See https://aka.ms/new-console-template for more information
using ServerLogAnalyzer;
using System.Diagnostics;
using System.Text.Json;

var settings = new AppSettings();
try
{
    settings = JsonSerializer.Deserialize(File.ReadAllText("AppSettings.json"), AppSettingsContext.Default.AppSettings)!;
}
catch (JsonException e)
{
    Console.WriteLine("AppSettings.json is not valid JSON");
    Console.WriteLine(e.Message);
    Console.WriteLine("Press any key to exit");
    Console.ReadKey();
    return;
}
catch (FileNotFoundException e)
{
    // ignore since it's not a mandatory file
}

// get input directory
Console.WriteLine($"Running with settings: {settings}");
Console.WriteLine();
Console.WriteLine($"Give the complete path to the directory of the log files or leave empty to use the InputDirectory from AppSettings.json ({settings.InputDirectory}). Leave the InputDirectory in AppSettings.json empty to use current directory by default.");
var inputDir = Console.ReadLine();
if (string.IsNullOrWhiteSpace(inputDir))
{
    inputDir = string.IsNullOrWhiteSpace(settings?.InputDirectory)
        ? AppDomain.CurrentDomain.BaseDirectory
        : settings.InputDirectory;
}

inputDir = inputDir.Trim();

if (inputDir.Last() != '\\')
{
    inputDir += "\\"; // add trailing slash
}

// get output directory
Console.WriteLine($"Give the complete path to the directory of the CSV output or leave empty to use the OutputDirectory from AppSettings.json ({settings!.OutputDirectory}). Leave the OutputDirectory in AppSettings.json empty to use current directory by default.");
var outputDir = Console.ReadLine();
if (string.IsNullOrWhiteSpace(outputDir))
{
    outputDir = string.IsNullOrWhiteSpace(settings?.OutputDirectory)
        ? AppDomain.CurrentDomain.BaseDirectory
        : settings.OutputDirectory;
}

outputDir = outputDir.Trim();

if (outputDir.Last() != '\\')
{
    outputDir += "\\"; // add trailing slash
}

// Get number of files analyzed
Console.WriteLine($"Give the maximum number of log files to analyze or leave empty to use the MaxNumberOfFiles from AppSettings.json ({settings!.MaxNumberOfFiles}). The files are ordered by date of last modification.");
var maxFileCount = settings.MaxNumberOfFiles;
var maxFileCountInput = Console.ReadLine();
if (!string.IsNullOrWhiteSpace(maxFileCountInput) && int.TryParse(maxFileCountInput, out var parsedMaxFileCount))
{
    maxFileCount = parsedMaxFileCount;
}

// Read Data
var loggedDates = LogReader.ReadLogsFromDirectory(inputDir, maxFileCount);

// Write CSVs
var fileNames = new List<string>
{
    CSVWriter.WriteViewDocumentRequestTimesPerDate(loggedDates, outputDir),
    CSVWriter.WriteSIIRTORekisteriRequestCountPerDate(loggedDates, settings.IgnoredFileEndings, outputDir),
    CSVWriter.WriteApplicationAccessCountAndTimeAveragePerRequest(loggedDates, settings.IgnoredFileEndings, outputDir),
};
Console.WriteLine("Done!");
Console.WriteLine("Press Y to close and open the CSV files");
Console.WriteLine("Press N to close without opening the files");

var key = Console.ReadKey();
if (key.Key == ConsoleKey.Y)
{
    foreach (var fileName in fileNames)
    {
        new Process()
        {
            StartInfo = new ProcessStartInfo { FileName = outputDir + fileName, UseShellExecute = true }
        }.Start();
    }
}