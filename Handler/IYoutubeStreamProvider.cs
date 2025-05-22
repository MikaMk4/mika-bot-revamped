using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Handler
{
    internal interface IYoutubeStreamProvider
    {
        Task<YoutubeVideoInstance> GetStreamAsync(YoutubeVideoInstance youtubeVideoInstance);
    }
}
