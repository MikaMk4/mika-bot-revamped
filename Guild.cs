using Discord;
using Discord.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    internal class Guild
    {
        private readonly ulong id = 0;
        public List<GuildUser> Users { get; }
        public IVoiceChannel? BotVoiceChannel { get; set; }

        private IAudioClient? botAudioClient;
        public IAudioClient? BotAudioClient
        {
            get => botAudioClient;
            set
            {
                if (value != null)
                {
                    if (botAudioClient != null)
                    {
                        botAudioClient.Disconnected -= Disconnected;
                    }

                    botAudioClient = value;
                    botAudioClient.Disconnected += Disconnected;
                } else
                {
                    botAudioClient = null;
                }
            }
        }

        public Guild(ulong id)
        {
            this.id = id;
            Users = new List<GuildUser>();
        }

        public Task Disconnected(Exception e)
        {
            Program.Log(LogSeverity.Info, $"Guild {id}", "Force-disconnected from voice channel.");
            BotAudioClient = null;
            BotVoiceChannel = null;
            return Task.CompletedTask;
        }
    }
}
