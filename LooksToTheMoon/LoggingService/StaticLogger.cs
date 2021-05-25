using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using ConsoleColour = System.ConsoleColor;

namespace LooksToTheMoon.LoggingService {
    public static class StaticLogger {

        public static Dictionary<LogSeverity, ConsoleColour> ColourSeverityMatch = new Dictionary<LogSeverity, ConsoleColor> {
            {LogSeverity.Debug, ConsoleColour.DarkCyan},
            {LogSeverity.Verbose, ConsoleColour.Gray},
            {LogSeverity.Info, ConsoleColour.White},
            {LogSeverity.Warning, ConsoleColour.Yellow},
            {LogSeverity.Error, ConsoleColour.Red},
            {LogSeverity.Critical, ConsoleColour.DarkRed}
        };
        
        public static async Task FormatLogAsync(LogMessage logMessage) {
            
            ConsoleColour colour = ColourSeverityMatch[logMessage.Severity];
            Console.ForegroundColor = colour;

            await Console.Out.WriteLineAsync($"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss\\.ffff} | {logMessage.Severity} | {logMessage.Source}] {logMessage.Message}");
            
            Console.ResetColor();
        }

        public static async Task FormatExceptionAsync(Exception exception) {
            await FormatLogAsync(new LogMessage(LogSeverity.Error, "Exception Service", exception.Message));
        }

    }
}