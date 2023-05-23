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

            client.Log += Log;
            client.Ready += RegisterCommands;
            client.SlashCommandExecuted += commandHandler.SlashCommandHandler;
        }

        public async Task RegisterCommands()
        {
            await Log(new LogMessage(LogSeverity.Info, "Bot", "Registering Commands"));

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
                    .WithName("play")
                    .WithDescription("Play a local song.")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("local")
                        .WithDescription("Path to the local song.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("path")
                            .WithDescription("Path to the local song.")
                            .WithType(ApplicationCommandOptionType.String)
                            .WithRequired(true)
                        )
                        )
                    )
                ;
            applicationCommands.Add(musicCommand.Build());

            // JOIN COMMAND
            var joinCommand = new SlashCommandBuilder()
                .WithName("join")
                .WithDescription("Let Mika-Bot join a voice channel")
                .AddOption("channel", ApplicationCommandOptionType.Channel, "Voice channel to join.");
            applicationCommands.Add(joinCommand.Build());

            try
            {
                await guild.BulkOverwriteApplicationCommandAsync(applicationCommands.ToArray());
            } catch (ApplicationCommandException e)
            {
                await Log(new LogMessage(LogSeverity.Error, e.Source, e.Message));
            }
        }

        public async Task Start()
        {
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        public Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}