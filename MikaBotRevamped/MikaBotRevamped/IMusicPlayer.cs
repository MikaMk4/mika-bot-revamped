using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    internal interface IMusicPlayer
    {
        Task QueueLocalAudioFile(string path);
        Task Play();
        Task StopPlayback();
        Task ResumePlayback();
    }
}
