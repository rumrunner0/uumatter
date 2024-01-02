using System;
using System.Text;
using Rumrunner0.UuMatter.Console;
using Serilog;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

Log.Logger = UuMatter.OfType<ILogger>();
var settings = UuMatter.OfType<UuSettings>();
var logger = Log.Logger.ForContext<Program>();
logger.Information("Application has been started");

const string keyName = "PingKey";
Console.WriteLine($"{keyName}: {settings.Value(key: new (keyName))}");

logger.Information("Application has been shut down");
logger.Information("");
Log.CloseAndFlush();