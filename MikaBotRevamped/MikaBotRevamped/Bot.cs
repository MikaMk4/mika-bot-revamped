using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace MikaBotRevamped
{
    internal class Bot
    {
        private DiscordSocketClient client;
        private string? token;

        public Bot(string? token)
        {
            var config = new DiscordSocketConfig
            {
                UseInteractionSnowflakeDate = false
            };

            client = new DiscordSocketClient(config);

            this.token = token;

            CommandHandler commandHandler = new CommandHandler(client);

            client.Log += Program.Log;
            client.Ready += RegisterCommands;
            client.SlashCommandExecuted += commandHandler.SlashCommandHandler;
        }

        public async Task RegisterCommands()
        {
            await Program.Log(new LogMessage(LogSeverity.Info, "Bot", "Registering Commands"));

            var guild = client.GetGuild(870773459104436245); // Mondstadt Server

            List<ApplicationCommandProperties> applicationCommands = new();

            // ROLES COMMAND
            var rolesCommand = new SlashCommandBuilder()
                .WithName("roles")
                .WithDescription("List all roles of a given user.")
                .AddOption("user", ApplicationCommandOptionType.User, "User of which to show roles.", isRequired: true);
            applicationCommands.Add(rolesCommand.Build());

            // MUSIC COMMAND
            var musicCommand = new SlashCommandBuilder()
                .WithName("music")
                .WithDescription("Play music in a voice channel.")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("queue")
                    .WithDescription("Queue a song.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                            .WithName("url")
                            .WithDescription("Get video through an URL")
                            .WithType(ApplicationCommandOptionType.SubCommand)
                            .AddOption(new SlashCommandOptionBuilder()
                                .WithName("input")
                                .WithDescription("The URL to the video.")
                                .WithType(ApplicationCommandOptionType.String)
                                .WithRequired(true)
                            )
                     )
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("search-query")
                        .WithDescription("Get a video through a search.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("input")
                            .WithDescription("Search Query")
                            .WithType(ApplicationCommandOptionType.String)
                            .WithRequired(true)
                        )
                    )
                );

            applicationCommands.Add(musicCommand.Build());

            // JOIN COMMAND
            var joinCommand = new SlashCommandBuilder()
                .WithName("join")
                .WithDescription("Let Mika-Bot join a voice channel")
                .AddOption("channel", ApplicationCommandOptionType.Channel, "Voice channel to join.");
            applicationCommands.Add(joinCommand.Build());

            // LEAVE COMMAND
            var leaveCommand = new SlashCommandBuilder()
                .WithName("leave")
                .WithDescription("Let Mika-Bot leave the current voice channel");
            applicationCommands.Add(leaveCommand.Build());

            try
            {
                await guild.BulkOverwriteApplicationCommandAsync(applicationCommands.ToArray());
            } catch (ApplicationCommandException e)
            {
                await Program.Log(new LogMessage(LogSeverity.Error, e.Source, e.Message));
            }
        }

        public async Task Start()
        {
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }
    }
}