using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    [Serializable]
    public class User
    {
        public ulong Uid { get; set; }
        public int PityCount { get; set; } = 0;
        public List<Waifu> Waifus { get; set; } = new List<Waifu>();
        public List<UnclaimedWaifu> UnclaimedWaifus { get; set; } = new List<UnclaimedWaifu>();

        [NonSerialized]
        public RestFollowupMessage? RestFollowupMessage;

        public User(ulong uid)
        {
            Uid = uid;
        }
    }
}
