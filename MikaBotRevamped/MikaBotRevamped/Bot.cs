using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using MikaBotRevamped.Dependencies;
using MikaBotRevamped.Handler;
using MikaBotRevamped.Items;
using System.Collections.Concurrent;

namespace MikaBotRevamped
{
    internal class Bot
    {
        private readonly DiscordSocketClient client;
        private readonly string? token;

        public ConcurrentDictionary<ulong, Guild> guilds = new();
        public UserDictionary Users;

        public SlashCommandHandler SlashCommandHandler { get; }
        private ButtonHandler ButtonHandler { get; }
        private ModalHandler ModalHandler { get; }
        private SelectMenuHandler SelectMenuHandler { get; }


        public Bot(string? token, IEnumerable<IButton> buttons)
        {
            var config = new DiscordSocketConfig
            {
                UseInteractionSnowflakeDate = false
            };

            client = new DiscordSocketClient(config);

            this.token = token;

            SlashCommandHandler = new(client);
            ButtonHandler = new(buttons);
            ModalHandler = new();
            SelectMenuHandler = new(client);

            client.Log += Program.Log;
            client.Ready += Ready;
            client.SlashCommandExecuted += SlashCommandHandler.HandleSlashCommand;
            client.SelectMenuExecuted += SelectMenuHandler.ResolveSelectMenu;
            client.ButtonExecuted += ButtonHandler.HandleButton;
            client.ModalSubmitted += ModalHandler.HandleModal;
            client.JoinedGuild += RegisterGuild;
        }

        private Task Ready()
        {
            InstantiateAllSlashCommands();
            //Program.RegisterAllSlashCommands();

            Program.Log(LogSeverity.Info, "Bot", "Loading all Users.");
            Users = new UserDictionary(new JsonPersistenceProvider("./"));
            Users.Load().Wait();

            client.SetStatusAsync(UserStatus.Online);

            client.SetGameAsync("Gacha Games!", type: ActivityType.Competing);
            return Task.CompletedTask;
        }

        private Task InstantiateAllSlashCommands()
        {
            SlashCommandHandler.ClearSlashCommands();

            // Hol alle Klassen die ISlashCommand implementieren
            var slashCommandTypes = Program.GetImplementingTypes(typeof(ISlashCommand));
            // Instanziere diese Klassen und pack sie in eine Liste
            var slashCommands = slashCommandTypes.Select(Program.CreateInstance).Cast<ISlashCommand>().ToList();

            // Setze die Dependencies für die SlashCommands und Buttons
            slashCommands.ForEach(slashCommand => slashCommand.SetDependencies(Program.dependencyProvider));

            foreach (var slashCommand in slashCommands)
            {
                SlashCommandHandler.AddSlashCommand(slashCommand);
            }

            return Task.CompletedTask;
        }

        public async Task RegisterUser(ulong uid)
        {
            var dUser = await client.GetUserAsync(uid);
            var user = new User(uid);
            user.AddItem(new Roll(), 20);
            Users.TryAdd(user);
        }

        private async Task RegisterGuild(SocketGuild socketGuild)
        {
            var guild = new Guild(socketGuild.Id);
            guilds.TryAdd(socketGuild.Id, guild);
        }

        public async Task<(User owner, Waifu waifu)> SearchUserAndWaifuByWaifuId(ulong targetWaifuId)
        {
            foreach (var userEntry in Users.Users.Values)
            {
                foreach (var waifu in userEntry.Waifus)
                {
                    if (waifu.Id == targetWaifuId)
                    {
                        return (userEntry, waifu);
                    }
                }
            }

            return (null, null);
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