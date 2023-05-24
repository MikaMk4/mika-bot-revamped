﻿using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System.Collections;

namespace MikaBotRevamped
{
    internal partial class CommandHandler
    {
        private DiscordSocketClient client;

        private Dictionary<ulong?, IVoiceChannel> voiceChannelInGuild = new();
        private Dictionary<ulong?, IAudioClient> audioClientInGuild = new();

        public CommandHandler(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            // Neue Commands hier hinzufügen

            await command.DeferAsync();

            switch (command.Data.Name)
            {
                case "roles":
                    await ListRolesOfUserCommand(command);
                    break;
                case "music":
                    MusicCommand(command);
                    break;
                case "join":
                    JoinVoiceChannel(command);
                    break;
                case "leave":
                    LeaveVoiceChannel(command);
                    break;
                default:
                    await command.FollowupAsync("Oops, something went wrong. (default case)");
                    break;
            }
        }

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
                    await command.FollowupAsync("Can only join voice channels.");
                    return;
                }
            } else
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
            voiceChannelInGuild.Add(command.GuildId, voiceChannel);
            audioClientInGuild.Add(command.GuildId, audioClient);
        }

        private async Task LeaveVoiceChannel(SocketSlashCommand command)
        {
            if (!voiceChannelInGuild.ContainsKey(command.GuildId))
            {
                await command.FollowupAsync("Mika-Bot is currently not in a voice channel.");
            } else
            {
                await voiceChannelInGuild[command.GuildId].DisconnectAsync();
                await command.FollowupAsync($"Left {voiceChannelInGuild[command.GuildId]}");
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

            await command.FollowupAsync(embed: embed);
        }
    }
}