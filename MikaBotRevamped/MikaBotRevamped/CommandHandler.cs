using Discord;
using Discord.WebSocket;

namespace MikaBotRevamped
{
    internal class CommandHandler
    {
        private DiscordSocketClient client;

        private HashSet<ulong> registeredGuilds = new();

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
                    await MusicCommand(command);
                    break;
                case "join":
                    await JoinVoiceChannel(command);
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

            await command.FollowupAsync($"Joining voice channel {voiceChannel.Name}");
            await voiceChannel.ConnectAsync();
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

        private async Task MusicCommand(SocketSlashCommand command)
        {
            await command.FollowupAsync("Music command is not yet implemented.");
        }
    }
}