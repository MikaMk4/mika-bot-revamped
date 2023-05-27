using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    internal class MusicCommand : ISlashCommand
    {
        public string CommandName => "music";
        public bool AsyncMode => false;

        private IYoutubeUrlProvider youtubeUrlProvider;

        public void SetDepencies(IDependencyProvider dependencyProvider)
        {
            youtubeUrlProvider = dependencyProvider.GetDependency<IYoutubeUrlProvider>();
        }

        public async Task Respond(SocketSlashCommand command)
        {
            var subCommand = command.Data.Options.First().Name;

            if (subCommand == "search")
            {
                var searchQuery = command.Data.Options.First().Options.First().Value as string;
                var count = command.Data.Options.First().Options.Last().Value as int? ?? 5;
                if (searchQuery == null)
                {
                    await command.FollowupAsync("Oops, something went wrong. (search)");
                    await Program.Log(LogSeverity.Error, "CommandHandler", "searchQuery is null");
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
                    selectMenuBuilder.AddOption(youtubeVideoInstance.ChannelTitle, $"opt-{index + 1}", youtubeVideoInstance.Title);
                }

                var componentBuilder = new ComponentBuilder()
                    .WithSelectMenu(selectMenuBuilder);

                var followupMessage = await command.FollowupAsync(embed: embedBuilder.Build(), components: componentBuilder.Build());
                var user = new GuildUser(command.User.Id);
                user.FollowupMessage = followupMessage;
                Program.bot.guilds[(ulong)command.GuildId].Users.Add(user);
                return;
            }

            await command.FollowupAsync(subCommand.ToString());
        }

        public SlashCommandProperties GetCommandProperties()
        {
            return new SlashCommandBuilder()
                .WithName("music")
                .WithDescription("Play music in a voice channel.")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("queue")
                    .WithDescription("Queue a song.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("search")
                    .WithDescription("Search for song.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("query")
                        .WithDescription("Search query.")
                        .WithType(ApplicationCommandOptionType.String)
                        .WithRequired(true)
                    )
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("count")
                        .WithDescription("Number of results to search for.")
                        .WithType(ApplicationCommandOptionType.Integer)
                    )
                ).Build();
        }
    }
}
