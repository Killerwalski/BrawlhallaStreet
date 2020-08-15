using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Collections.Generic;
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
        [Summary("Make the bot recap the last game span")]
        // [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Recap()
        {
            var allPlayerRecaps = await StreetBot.GetPlayerSummaries();
            if (allPlayerRecaps.Count == 0)
            {
                await ReplyAsync("No games to update!");
                return;
            }
            
            //var recapBuilders = FormatRecaps(allPlayerRecaps.FirstOrDefault());
            //foreach (var item in recapBuilders)
            //{
            //    await ReplyAsync("", false, item.Build());
            //}


            foreach (var playerRecap in allPlayerRecaps)
            {
                var recapBuilders = FormatRecaps(playerRecap);
                foreach (var builder in recapBuilders)
                {
                    await ReplyAsync("", false, builder.Build());
                    // TODO make usre we're not spamming
                }
            }

        }

        //[Command("testEmbed")]
        [Summary("Dev feature")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task TestEmbed()
        {
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Title = ""
            };
            for (int i = 0; i < 10; i++)
            {
                builder.AddField(x =>
                {
                    x.Name = "Count";
                    x.Value = i;

                    x.IsInline = true;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }

        public List<EmbedBuilder> FormatRecaps(List<StatsSummary> statsSummaries)
        {
            var builders = new List<EmbedBuilder>();

            var builder = new EmbedBuilder()
            {
                Color = new Color(222, 137, 218),
                Title = "Legend Summaries for Player: " + statsSummaries.FirstOrDefault().PlayerName
            };
            builder.AddField(x =>
            {
                x.Name = "Legends Played";
                x.Value = statsSummaries.Count;
            });
            builders.Add(builder);
            foreach (var legend in statsSummaries)
            {
                // Each legend should be its own builder
                builders.Add(FormatLegend(legend));
            }
            return builders;
        }

        public EmbedBuilder FormatLegend(StatsSummary statsSummary)
        {
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Title = statsSummary.LegendName
            };
            var count = 0;
            var summaryProps = typeof(StatsSummary).GetProperties().Where(x => x.Name != "LegendName" && x.Name != "PlayerName").ToList();
            foreach (var property in summaryProps)
            {
                count = summaryProps.IndexOf(property);
                if (count == 0)
                    count++;
                builder.AddField(x =>
                {
                    x.Name = property.Name;
                    x.Value = property.GetValue(statsSummary, null);
                    x.IsInline = true; 
                });
            }

            return builder;
        }

        public string FormatRecapsOld(List<StatsSummary> recaps)
        {
            string output = string.Empty;
            recaps.ForEach(x => output += x.ToString());
            if (output == string.Empty)
                output += "No games to recap!";
            if (output.Length > 2000)
            {
                output = output.Substring(0, 1990) + "[TRIMMED]";
            }
            return output;
        }
    }
}