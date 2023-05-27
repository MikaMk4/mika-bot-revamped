using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    internal class JoinCommand : ISlashCommand
    {
        public string CommandName => "join";
        public bool AsyncMode => true;
        public void SetDepencies(IDependencyProvider dependencyProvider)
        {
        }
        public async Task Respond(SocketSlashCommand command)
        {
            IVoiceChannel voiceChannel;

            if (command.Data.Options.Any())
            {
                if (command.Data.Options.First().Value is IVoiceChannel)
                {
                    voiceChannel = command.Data.Options.First().Value as IVoiceChannel;
                }
                else
                {
                    await command.FollowupAsync("Can only join voice channels.");
                    return;
                }
            }
            else
            {
                voiceChannel = (command.User as IGuildUser).VoiceChannel;
            }

            if (voiceChannel == null)
            {
                await command.FollowupAsync("You are not in a voice channel.");
                return;
            }

            var audioClient = await voiceChannel.ConnectAsync();
            await command.FollowupAsync($"Joined voice channel '{voiceChannel.Name}'");
            var guild = Program.bot.guilds[(ulong)command.GuildId];
            guild.BotVoiceChannel = voiceChannel;
            guild.BotAudioClient = audioClient;
        }
        public SlashCommandProperties GetCommandProperties()
        {
            return new SlashCommandBuilder()
                .WithName("join")
                .WithDescription("Let Mika-Bot join a voice channel")
                .AddOption("channel", ApplicationCommandOptionType.Channel, "Voice channel to join.")
                .Build();
        }
    }
}
