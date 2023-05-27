using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    internal interface IYoutubeUrlProvider
    {

        List<YoutubeVideoInstance> GetAudioInstancesFromSearch(string searchQuery, int count);
    }
}
