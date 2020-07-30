using BrawlhallaStreet.Core.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrawlhallaStreet.Core
{
    public class StreetBot
    {
        private DiscordSocketClient Client;
		private readonly IConfiguration Configuration;
		private IDataService DataService;
		public ILogger Logger;

		public StreetBot(IConfiguration configuration, IDataService dataService)
		{
			Configuration = configuration;
			DataService = dataService;
		}
		public async Task MainAsync()
		{
			await SetupLogger();
			Client = new DiscordSocketClient();
			Client.Log += Log;

			// Remember to keep token private or to read it from an 
			// external source! In this case, we are reading the token 
			// from an environment variable. If you do not know how to set-up
			// environment variables, you may find more information on the 
			// Internet or by using other methods such as reading from 
			// a configuration.
			await Client.LoginAsync(TokenType.Bot,
				Configuration["DiscordToken"]);
				// Environment.GetEnvironmentVariable("DiscordToken"));
				// "NzM3MTU4OTY5MTg1MDA5NzQ2.Xx5Syg.vn617f-LfhlHAIHAFzbf40Kr1yk");
			await Client.StartAsync();

			// Block this task until the program is closed.
			// await Task.Delay(-1);

			await Client.StopAsync();
			Client.Dispose();
		}

		private Task SetupLogger()
        {
			Logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.CreateLogger()
				.ForContext<StreetBot>();

			return Task.CompletedTask;
		}

		private Task Log(LogMessage msg)
        {
			Logger.Information(msg.ToString());
            // Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
