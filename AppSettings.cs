using System.Text.Json.Serialization;

namespace ServerLogAnalyzer;
public record AppSettings
{
    public string InputDirectory { get; set; } = "";
    public string OutputDirectory { get; set; } = "";
    public int MaxNumberOfFiles { get; set; } = 100;
}

[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(int))] // etc.. etc.. https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation?pivots=dotnet-6-0
public partial class AppSettingsContext : JsonSerializerContext
{
}