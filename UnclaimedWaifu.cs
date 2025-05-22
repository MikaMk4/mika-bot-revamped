using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    public class UnclaimedWaifu
    {
        public string ImageUrl { get; set; }
        public WaifuQuality Quality { get; set; }
        public UnclaimedWaifu(string imageUrl)
        {
            ImageUrl = imageUrl;
        }
    }
}
