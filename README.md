Analyzes IIS log files to output data I care about as CSV

# Usage
Run dotnet publish -c Release in the folder with the csproj.
Fetch the .exe from "ServerLogAnalyzer\bin\Release\net8.0\win-x64\publish"
Run the .exe and give it the location of the log files as input. Or put the location in AppSettings.json in the same folder as the .exe.
