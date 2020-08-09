using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace BrawlhallaStreet.Core.Modules
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService CommandService;
        private readonly IConfigurationRoot Configuration;

        public CommandModule(CommandService service, IConfigurationRoot config)
        {
            CommandService = service;
            Configuration = config;
        }

        [Command("recap"), Alias("r")]
        [Summary("Make the bot recap the last game")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public Task Recap([Remainder] string text)
        {
            return ReplyAsync(text);
        }
    }
}