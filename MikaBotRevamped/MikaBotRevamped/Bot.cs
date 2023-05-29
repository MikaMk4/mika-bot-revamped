﻿using Discord;
using Discord.Net;
using Discord.WebSocket;
using MikaBotRevamped.Dependencies;
using MikaBotRevamped.Handler;
using System.Collections.Concurrent;

namespace MikaBotRevamped
{
    internal class Bot
    {
        private readonly DiscordSocketClient client;
        private readonly string? token;

        public ConcurrentDictionary<ulong, Guild> guilds = new();
        public readonly UserDictionary Users;

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

            Users = new UserDictionary(new JsonPersistenceProvider("Users.json"));
            Users.Load().Wait();

            guilds.TryAdd(870773459104436245, new Guild(870773459104436245));

            client.Log += Program.Log;
            client.Ready += SlashCommandHandler.RegisterCommands;
            client.Ready += Ready;
            client.SlashCommandExecuted += SlashCommandHandler.HandleSlashCommand;
            client.SelectMenuExecuted += SelectMenuHandler.ResolveSelectMenu;
            client.ButtonExecuted += ButtonHandler.HandleButton;
            client.ModalSubmitted += ModalHandler.HandleModal;
            client.JoinedGuild += RegisterGuild;
        }

        private Task Ready()
        {
            Program.RegisterAllSlashCommands();
            client.SetGameAsync("Gacha Games!", type: ActivityType.Competing);
            return Task.CompletedTask;
        }

        public void RegisterUser(ulong uid)
        {
            var user = new User(uid);
            Users.TryAdd(uid, user);
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