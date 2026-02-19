using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Flashcards._0lcm.Logging;

internal class CustomFormatter : ConsoleFormatter
{
    public CustomFormatter() : base("customFormatter")
    {
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter
    )
    {
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (string.IsNullOrEmpty(message)) return;

        var originalColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = GetLogLevelColor(logEntry.LogLevel);

            textWriter.Write($"[{DateTimeOffset.Now:HH:mm:ss}]");

            textWriter.Write($"[{logEntry.LogLevel,-12}]");

            textWriter.Write($"[{logEntry.Category}]");

            textWriter.Write(message);

            if (logEntry.Exception != null) textWriter.Write(logEntry.Exception.ToString());
        }
        finally
        {
            Console.ForegroundColor = originalColor;
        }
    }

    private static ConsoleColor GetLogLevelColor(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => ConsoleColor.Gray,
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Information => ConsoleColor.Green,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.Magenta,
            _ => ConsoleColor.White
        };
    }
}

internal class AppLogger
{
    private static readonly ILoggerFactory AppLoggerFactory =
        LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Debug)
                .AddConsole(options => { options.FormatterName = "customFormatter"; })
                .AddConsoleFormatter<CustomFormatter, ConsoleFormatterOptions>();
        });

    internal static ILogger CreateLogger<T>()
    {
        return AppLoggerFactory.CreateLogger<T>();
    }
}