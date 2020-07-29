using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrawlhallaStreet.Core
{
    public class StreetBot
    {
        private DiscordSocketClient _client;

		public async Task MainAsync()
		{
			_client = new DiscordSocketClient();

			_client.Log += Log;

			// Remember to keep token private or to read it from an 
			// external source! In this case, we are reading the token 
			// from an environment variable. If you do not know how to set-up
			// environment variables, you may find more information on the 
			// Internet or by using other methods such as reading from 
			// a configuration.
			await _client.LoginAsync(TokenType.Bot,
				// Environment.GetEnvironmentVariable("DiscordToken"));
				"NzM3MTU4OTY5MTg1MDA5NzQ2.Xx5Syg.vn617f-LfhlHAIHAFzbf40Kr1yk");
			await _client.StartAsync();

			// Block this task until the program is closed.
			// await Task.Delay(-1);

			await _client.StopAsync();
			_client.Dispose();
		}


		private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
