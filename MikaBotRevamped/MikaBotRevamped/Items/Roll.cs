using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Items
{
    internal class Roll : IGameItem
    {
        public int Id { get; set; } = 1;
        public string Name { get; set; } = "Roll";
        public string Description { get; } = "Used for rolling a single Waifu.";
        public Discord.Emoji Emoji { get; } = new Discord.Emoji("🎲");

        public Roll()
        {
        }
    }
}
