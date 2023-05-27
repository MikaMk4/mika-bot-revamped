using Discord;
using Discord.Net;
using Discord.WebSocket;
using System.Collections.Concurrent;

namespace MikaBotRevamped
{
    internal class Bot
    {
        private readonly DiscordSocketClient client;
        private readonly string? token;

        public ConcurrentDictionary<ulong, Guild> guilds = new();

        public Bot(string? token, IEnumerable<ISlashCommand> slashCommands)
        {
            var config = new DiscordSocketConfig
            {
                UseInteractionSnowflakeDate = false
            };

            client = new DiscordSocketClient(config);

            this.token = token;

            CommandHandler commandHandler = new(client, slashCommands);
            SelectMenuHandler selectMenuHandler = new(client);

            guilds.TryAdd(870773459104436245, new Guild(870773459104436245));

            client.Log += Program.Log;
            client.Ready += commandHandler.RegisterCommands;
            client.SlashCommandExecuted += commandHandler.HandleSlashCommand;
            client.SelectMenuExecuted += selectMenuHandler.ResolveSelectMenu;
            client.JoinedGuild += RegisterGuild;
        }

        private async Task RegisterGuild(SocketGuild socketGuild)
        {
            var guild = new Guild(socketGuild.Id);
            guilds.TryAdd(socketGuild.Id, guild);
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

            // LEAVE COMMAND
            var leaveCommand = new SlashCommandBuilder()
                .WithName("leave")
                .WithDescription("Let Mika-Bot leave the current voice channel");
            applicationCommands.Add(leaveCommand.Build());

            try
            {
                await guild.BulkOverwriteApplicationCommandAsync(applicationCommands.ToArray());
            }
            catch (ApplicationCommandException e)
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