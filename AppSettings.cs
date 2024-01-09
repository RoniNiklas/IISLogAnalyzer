using System.Text.Json.Serialization;

namespace ServerLogAnalyzer;
public class AppSettings
{
    public string InputDirectory { get; set; } = "";
    public string OutputDirectory { get; set; } = "";
}

[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(bool))] // etc.. etc.. https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation?pivots=dotnet-6-0
public partial class AppSettingsContext : JsonSerializerContext
{
}