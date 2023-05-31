using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MikaBotRevamped
{
    [Serializable]
    public class User
    {
        public string Username { get; }
        public ushort Discriminator { get; }
        public ulong Uid { get; }
        public int PityCount { get; set; } = 0;
        public List<Waifu> Waifus { get; set; } = new List<Waifu>();
        public List<UnclaimedWaifu> UnclaimedWaifus { get; set; } = new List<UnclaimedWaifu>();

        [NonSerialized]
        public RestFollowupMessage? RestFollowupMessage;

        [JsonConstructor]
        public User(ulong uid, string username, ushort discriminator)
        {
            Uid = uid;
            Username = username;
            Discriminator = discriminator;
        }
    }
}
