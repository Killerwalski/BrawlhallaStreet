using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrawlhallaStreet.Core.Services
{
    public class LoggingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly ILogger _logger;

        public LoggingService(DiscordSocketClient discord, CommandService commands, ILogger logger)
        {
            _discord = discord;
            _commands = commands;
            _logger = logger;

            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }
        private Task OnLogAsync(LogMessage msg)
        {
            string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
            _logger.Information(logText);

            return Console.Out.WriteLineAsync(logText);       // Write the log text to the console
        }
    }
}
