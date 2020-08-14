using BrawlhallaStreet.Core.Services;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Linq;
using System.Threading.Tasks;

namespace BrawlhallaStreet.Core.Modules
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService CommandService;
        private readonly IConfigurationRoot Configuration;
        private readonly ILogger Logger;
        private StreetBot StreetBot;

        public CommandModule(CommandService service, IConfigurationRoot config, ILogger logger, StreetBot streetBot)
        {
            CommandService = service;
            Configuration = config;
            Logger = logger;
            StreetBot = streetBot;
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            string prefix = "!";
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "These are the commands you can use"
            };

            foreach (var module in CommandService.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        description += $"{prefix}{cmd.Aliases.First()}\n";
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("recap"), Alias("r")]
        [Summary("Make the bot recap the last game")]
        // [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Recap()
        {
            var recaps = await StreetBot.GetPlayerSummaries();
            string output = string.Empty;
            foreach (var recap in recaps)
            {
                output += recap.ToString() + "\n";
            }
            if (output == string.Empty)
                output += "No games to recap!";
            if (output.Length > 2000)
            {
                output = output.Substring(0, 1990) + "[TRIMMED]";
            }
            await ReplyAsync(output);
        }

        public async Task<string> GetRecap()
        {
            var recaps = await StreetBot.GetPlayerSummaries();
            string output = string.Empty;
            foreach (var recap in recaps)
            {
                output += recap.ToString() + "\n";
            }
            return output;
        }
    }
}