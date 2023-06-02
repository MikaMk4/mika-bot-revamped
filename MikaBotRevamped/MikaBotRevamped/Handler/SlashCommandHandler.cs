using Discord.Audio;
using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikaBotRevamped.Handler;

namespace MikaBotRevamped
{
    internal partial class SlashCommandHandler
    {
        private List<ISlashCommand> slashCommands = new();

        private DiscordSocketClient client;
        private Dictionary<ulong?, IVoiceChannel> voiceChannelInGuild = new();
        private Dictionary<ulong?, IAudioClient> audioClientInGuild = new();

        public SlashCommandHandler(DiscordSocketClient client)
        {
            this.client = client;
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

            if (slashCommand.AsyncMode)
            {
                slashCommand.Respond(command);
            }
            else
            {
                await slashCommand.Respond(command);
            }
        }

        public async Task RegisterCommands(ulong guildId)
        {
            List<ApplicationCommandProperties> commandProperties = new();
            commandProperties.AddRange(slashCommands.Select(c => c.GetCommandProperties()));

            var guild = client.GetGuild(guildId);

            await guild.BulkOverwriteApplicationCommandAsync(commandProperties.ToArray());
        }

        internal async Task RegisterCommand(ulong guildId, ISlashCommand commandInstance)
        {
            var guild = client.GetGuild(guildId);

            await guild.CreateApplicationCommandAsync(commandInstance.GetCommandProperties());
        }

        internal async Task RegisterCommand(ISlashCommand commandInstance)
        {
            await client.CreateGlobalApplicationCommandAsync(commandInstance.GetCommandProperties());
        }

        public void AddSlashCommand(ISlashCommand slashCommand)
        {
            slashCommands.Add(slashCommand);
        }

        public void ClearSlashCommands()
        {
            slashCommands.Clear();
        }
    }
}
