using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    public class Waifu : UnclaimedWaifu
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public string[]? Tags { get; set; }

        private static ulong instanceCount = 0;

        public Waifu(string name, string imageUrl) : base(imageUrl)
        {
            Name = name;
            ImageUrl = imageUrl;
            instanceCount++;
            Id = instanceCount;
        }

        public Waifu(UnclaimedWaifu unclaimedWaifu, string name) : base(unclaimedWaifu.ImageUrl)
        {
            Name = name;
            ImageUrl = unclaimedWaifu.ImageUrl;
            Quality = unclaimedWaifu.Quality;
            instanceCount++;
            Id = instanceCount;
        }
        /// <summary>
        /// NICHT BENUTZEN! Wird nur für die Deserialisierung benötigt.
        /// </summary>
        public Waifu() : base("")
        {
            instanceCount++;
            Id = instanceCount;
        }
    }
}
