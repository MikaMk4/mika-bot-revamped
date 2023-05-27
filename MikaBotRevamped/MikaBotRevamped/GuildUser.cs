using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    internal class GuildUser
    {
        public RestFollowupMessage? FollowupMessage { get; set; }
        public ulong Id { get; }

        public GuildUser(ulong id)
        {
            Id = id;
        }
    }
}
