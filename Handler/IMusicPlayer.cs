using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Handler
{
    internal interface IMusicPlayer
    {
        Task Play();
        Task StopPlayback();
        Task ResumePlayback();
    }
}
