Analyzes IIS log files to output data I care about as CSV

# Usage
1. Run dotnet publish -c Release in the folder with the csproj.   
1. Fetch the .exe from "ServerLogAnalyzer\bin\Release\net8.0\win-x64\publish".   
1. Run the .exe and give it the location of the log files as input. Or put the location in AppSettings.json in the same folder as the .exe.   
