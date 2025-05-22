using Discord;
using Discord.WebSocket;
using MikaBotRevamped.Handler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Commands
{
    internal class MusicCommand : ISlashCommand
    {
        public string CommandName => "music";
        public bool AsyncMode => true;

        // Dependencies
        private IYoutubeUrlProvider youtubeUrlProvider;
        private IYoutubeStreamProvider youtubeStreamProvider;

        public void SetDependencies(IDependencyProvider dependencyProvider)
        {
            youtubeUrlProvider = dependencyProvider.GetDependency<IYoutubeUrlProvider>();
            youtubeStreamProvider = dependencyProvider.GetDependency<IYoutubeStreamProvider>();
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
                    await command.RespondAsync("Oops, something went wrong. (search)");
                    await Program.Log(LogSeverity.Error, "MusicCommand", "searchQuery is null");
                    return;
                }

                // Embed erstellen
                var youtubeVideoInstances = youtubeUrlProvider.GeVideoInstancesFromSearch(searchQuery, count);
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

                await command.DeferAsync();
                var followupMessage = await command.FollowupAsync(embed: embedBuilder.Build(), components: componentBuilder.Build());
                var user = new GuildUser(command.User.Id);
                user.FollowupMessage = followupMessage;
                Program.bot.guilds[(ulong)command.GuildId].Users.Add(user);
                return;
            }
            else if (subCommand == "url")
            {
                var url = command.Data.Options.First().Options.First().Value as string;
                if (url == null)
                {
                    await command.RespondAsync("Oops, something went wrong. (url)");
                    await Program.Log(LogSeverity.Error, "MusicCommand", "url is null");
                    return;
                }

                var youtubeVideoInstance = await youtubeStreamProvider.GetStreamAsync(youtubeUrlProvider.GetVideoInstanceFromUrl(url));

                var embedBuilder = new EmbedBuilder();
                embedBuilder.Title = "Chosen Song";
                embedBuilder.Description = $"You have chosen the song: {youtubeVideoInstance.Title}";
                embedBuilder.Color = Color.Green;
                await command.RespondAsync(embed: embedBuilder.Build());

                var discordStream = Program.bot.guilds[(ulong)command.GuildId].BotAudioClient.CreatePCMStream(Discord.Audio.AudioApplication.Mixed);
                try
                {
                    bool canWrite = true;

                    youtubeVideoInstance.AudioStream.Seek(0, SeekOrigin.Begin);

                    // Create a blocking collection to hold the buffers
                    BlockingCollection<byte[]> buffers = new BlockingCollection<byte[]>();

                    // Start a task to pre-load the buffers
                    Task.Run(() =>
                    {
                        while (true)
                        {
                            // Load the buffers and add them to the collection
                            byte[] buffer = new byte[4096 * 1000];
                            youtubeVideoInstance.AudioStream.Read(buffer, 0, buffer.Length);

                            if (buffer.Length == 0)
                            {
                                // No more buffers to load, so we're done
                                break;
                            }

                            buffers.Add(buffer);
                            youtubeVideoInstance.AudioStream.Seek(buffer.Length, SeekOrigin.Current);
                            Program.Log(LogSeverity.Debug, "MusicCommand", $"Loaded {buffer.Length} bytes into buffer.");
                        }

                        // Mark the collection as complete since we've added all the buffers
                        buffers.CompleteAdding();
                    });

                    await Task.Delay(1000);

                    // Start another task to pass the buffers to the API wrapper asynchronously
                    Task.Run(async () =>
                    {
                        foreach (byte[] buffer in buffers.GetConsumingEnumerable())
                        {
                            // Wait for the canWrite flag to be true
                            while (!canWrite)
                            {
                                await Task.Delay(1);
                            }

                            canWrite = false;

                            await discordStream.WriteAsync(buffer, 0, buffer.Length);

                            canWrite = true;

                            Program.Log(LogSeverity.Debug, "MusicCommand", $"Sent {buffer.Length} bytes to API wrapper.");
                        }
                    });
                }
                catch (Exception e)
                {
                    Program.Log(LogSeverity.Error, "MusicCommand", e.Message);
                }
                finally
                {
                    discordStream.FlushAsync();
                }

                return;
            }

            await command.RespondAsync(subCommand.ToString());
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
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("url")
                    .WithDescription("Queue a song from a url.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("url")
                        .WithDescription("Url to the YouTube video to queue.")
                        .WithType(ApplicationCommandOptionType.String)
                        .WithRequired(true)
                    )
                )
                .Build();
        }
    }
}
