using Discord;
using MikaBotRevamped.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Dependencies
{
    internal class MusicPlayer : IMusicPlayer
    {
        private IVoiceChannel voiceChannel;
        private Queue<YoutubeVideoInstance> queue;

        public MusicPlayer(IVoiceChannel voiceChannel)
        {
            this.voiceChannel = voiceChannel;
        }

        async Task IMusicPlayer.Play()
        {
            throw new NotImplementedException();
        }

        async Task IMusicPlayer.ResumePlayback()
        {
            throw new NotImplementedException();
        }

        async Task IMusicPlayer.StopPlayback()
        {
            throw new NotImplementedException();
        }
    }
}
