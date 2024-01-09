using System.Text.Json.Serialization;

namespace ServerLogAnalyzer;
public record AppSettings(string InputDirectory = "", string OutputDirectory = "", int MaxNumberOfFiles = 100)
{
    public string[] IgnoredFileEndings { get; init; } = [".jpg", ".png", ".ico", ".gif", ".svg", ".otf", ".ttf", ".fnt", ".font", ".css", ".js", ".txt"];
}

[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(int))] // etc.. etc.. https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation?pivots=dotnet-6-0
public partial class AppSettingsContext : JsonSerializerContext
{
}