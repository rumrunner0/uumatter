using System;
using System.Text;
using Rumrunner0.UuMatter.Console;
using Serilog;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

var settings = UuMatter.OfType<UuSettings>();
Log.Logger = UuMatter.OfType<ILogger>();
Log.Logger.Information("Application has been started");

const string keyName = "PingKey";
Console.WriteLine($"{keyName}: {settings.Value(key: new (keyName))}");

Log.Logger.Information("Application has been shut down");
Log.Logger.Information("");
Log.CloseAndFlush();