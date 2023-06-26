using System;
using System.Text;
using Rumble.Essentials;
using Serilog;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

var settings = Essential.OfType<Settings>();
Log.Logger = Essential.OfType<ILogger>();

var logger = Log.Logger.ForContext<Program>();
logger.Information("Application has been started");

// ...

logger.Information("Application has been shut down");
logger.Information("");
Log.CloseAndFlush();