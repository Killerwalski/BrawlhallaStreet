using BrawlhallaStreet.Core.Services;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace BrawlhallaStreet.Core.Modules
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService CommandService;
        private readonly IConfigurationRoot Configuration;
        private readonly IDataService DataService;

        public CommandModule(CommandService service, IConfigurationRoot config)
        {
            CommandService = service;
            Configuration = config;
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
        public Task Recap()
        {
            var output = "Hello";
            return ReplyAsync(output);
        }

        //public Task Recap([Remainder] string text)
        //{

        //    return ReplyAsync(text);
        //}
    }
}