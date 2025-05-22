using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Handler
{
    internal interface IYoutubeUrlProvider
    {
        List<YoutubeVideoInstance> GeVideoInstancesFromSearch(string searchQuery, int count);
        YoutubeVideoInstance GetVideoInstanceFromUrl(string url);
    }
}
