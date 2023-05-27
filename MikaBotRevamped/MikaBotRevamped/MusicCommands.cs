using Discord;
using Discord.WebSocket;
using DotNetTools.SharpGrabber.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    internal partial class CommandHandler
    {
        private async Task MusicCommand(SocketSlashCommand command)
        {
            var subCommand = command.Data.Options.First().Name;

            if (subCommand == "search")
            {
                var searchQuery = command.Data.Options.First().Options.First().Value as string;
                var count = command.Data.Options.First().Options.Last().Value as int? ?? 5;
                if (searchQuery == null)
                {
                    await command.FollowupAsync("Oops, something went wrong. (search)");
                    Program.Log(LogSeverity.Error, "CommandHandler", "searchQuery is null");
                    return;
                }

                

                // Embed erstellen
                var youtubeVideoInstances = youtubeUrlProvider.GetAudioInstancesFromSearch(searchQuery, count);
                var embedBuilder = new EmbedBuilder();
                embedBuilder.Title = "Search results";
                embedBuilder.Description = $"Found {youtubeVideoInstances.Count} videos with query '{searchQuery}'";
                embedBuilder.Color = Color.Green;
                foreach (YoutubeVideoInstance youtubeVideoInstance in youtubeVideoInstances)
                {
                    embedBuilder.AddField(youtubeVideoInstance.Title, youtubeVideoInstance.ChannelTitle);
                }

                // Select Menu erstellen
                var selectMenuBuilder = new SelectMenuBuilder();
                selectMenuBuilder.CustomId = "searchResultSelectMenu";
                selectMenuBuilder.Placeholder = "Select a result";
                selectMenuBuilder.MinValues = 1;
                selectMenuBuilder.MaxValues = 1;
                foreach (var (youtubeVideoInstance, index) in youtubeVideoInstances.WithIndex())
                {
                    selectMenuBuilder.AddOption($"opt-{index}", youtubeVideoInstance.ChannelTitle);
                }

                var componentBuilder = new ComponentBuilder()
                    .WithSelectMenu(selectMenuBuilder);

                await command.FollowupAsync(embed: embedBuilder.Build(), components: componentBuilder.Build());
                return;
            }

            await command.FollowupAsync(subCommand.ToString());
        }
    }
}
