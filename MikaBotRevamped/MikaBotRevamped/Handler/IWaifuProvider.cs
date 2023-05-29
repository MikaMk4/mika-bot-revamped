using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Handler
{
    internal interface IWaifuProvider
    {
        public Task<string> GetRandomWaifuImageUrl();
        public Task<string> GetWaifuImageUrlByTag(string tag);
    }
}
