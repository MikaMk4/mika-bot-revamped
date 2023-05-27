﻿using Discord.Audio;
using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    internal partial class CommandHandler
    {
        private List<ISlashCommand> slashCommands = new();

        private DiscordSocketClient client;
        private Dictionary<ulong?, IVoiceChannel> voiceChannelInGuild = new();
        private Dictionary<ulong?, IAudioClient> audioClientInGuild = new();

        public CommandHandler(DiscordSocketClient client, IEnumerable<ISlashCommand> commands)
        {
            this.client = client;
            slashCommands.AddRange(commands);
        }

        public async Task HandleSlashCommand(SocketSlashCommand command)
        {
            ISlashCommand slashCommand = slashCommands.Find(c => c.CommandName.Equals(command.CommandName));

            if (slashCommand == null)
            {
                await command.RespondAsync("Oops, something went wrong. (slashCommand == null)");
                await Program.Log(LogSeverity.Error, "CommandHandler","SlashCommand not found: " + command.CommandName);
                return;
            }

            await command.DeferAsync();

            if (slashCommand.AsyncMode)
            {
                slashCommand.Respond(command);
            }
            else
            {
                await slashCommand.Respond(command);
            }
        }

        public async Task RegisterCommands()
        {
            List<ApplicationCommandProperties> commandProperties = new();
            commandProperties.AddRange(slashCommands.Select(c => c.GetCommandProperties()));

            var guild = client.GetGuild(870773459104436245); // Mondstadt Server

            await guild.BulkOverwriteApplicationCommandAsync(commandProperties.ToArray());
        }

        public void AddSlashCommand(ISlashCommand slashCommand)
        {
            slashCommands.Add(slashCommand);
        }
    }
}
