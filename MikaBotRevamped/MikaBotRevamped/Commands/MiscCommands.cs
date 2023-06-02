using Discord;
using Discord.Audio;
using Discord.WebSocket;
using MikaBotRevamped.Handler;
using System.Collections;

namespace MikaBotRevamped
{
    internal partial class SlashCommandHandler
    {
        private async Task JoinVoiceChannel(SocketSlashCommand command)
        {
            IVoiceChannel voiceChannel;

            if (command.Data.Options.Any())
            {
                if (command.Data.Options.First().Value is IVoiceChannel)
                {
                    voiceChannel = command.Data.Options.First().Value as IVoiceChannel;
                } else
                {
                    await command.RespondAsync("Can only join voice channels.");
                    return;
                }
            } else
            {
                voiceChannel = (command.User as IGuildUser).VoiceChannel;
            }

            if (voiceChannel == null)
            {
                await command.RespondAsync("You are not in a voice channel.");
                return;
            }

            var audioClient = await voiceChannel.ConnectAsync();
            await command.RespondAsync($"Joined voice channel '{voiceChannel.Name}'");
            voiceChannelInGuild.Add(command.GuildId, voiceChannel);
            audioClientInGuild.Add(command.GuildId, audioClient);
        }

        private async Task LeaveVoiceChannel(SocketSlashCommand command)
        {
            if (!voiceChannelInGuild.ContainsKey(command.GuildId))
            {
                await command.RespondAsync("Mika-Bot is currently not in a voice channel.");
            } else
            {
                await voiceChannelInGuild[command.GuildId].DisconnectAsync();
                await command.RespondAsync($"Left {voiceChannelInGuild[command.GuildId]}");
                voiceChannelInGuild.Remove(command.GuildId);

                
            }
        }
        private async Task ListRolesOfUserCommand(SocketSlashCommand command)
        {
            var user = command.Data.Options.First().Value as SocketGuildUser;

            string roleList = string.Join("\n", user.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

            var embed = new EmbedBuilder()
                .WithAuthor(user.ToString(), user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithTitle("Roles:")
                .WithDescription(roleList)
                .WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .Build();

            await command.RespondAsync(embed: embed);
        }
    }
}