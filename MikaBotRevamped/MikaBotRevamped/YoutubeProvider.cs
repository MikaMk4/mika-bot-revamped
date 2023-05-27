using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeSearch;
using Discord;

namespace MikaBotRevamped
{
    internal class YoutubeProvider : IYoutubeStreamProvider, IYoutubeUrlProvider
    {
        public async Task<Stream> GetStreamAsync(YoutubeVideoInstance youtubeVideoInstance)
        {
            throw new NotImplementedException();
        }

        public List<YoutubeVideoInstance> GetAudioInstancesFromSearch(string searchQuery, int count)
        {
            var videoSearch = new VideoSearch();

            Program.Log(new LogMessage(LogSeverity.Info, "YoutubeProvider", $"Searching for {count} videos with query {searchQuery}"));
            List<VideoInformation> videoInformations = videoSearch.SearchQuery(searchQuery, count);

            List<YoutubeVideoInstance> result = new();

            foreach (VideoInformation videoInformation in videoInformations)
            {
                YoutubeVideoInstance youtubeVideoInstance = new YoutubeVideoInstance();

                youtubeVideoInstance.Title = videoInformation.Title;
                youtubeVideoInstance.Description = videoInformation.Description;
                youtubeVideoInstance.Url = new Uri(videoInformation.Url);
                youtubeVideoInstance.Thumbnail = new Uri(videoInformation.Thumbnail);
                //youtubeVideoInstance.Duration = TimeSpan.FromSeconds(videoInformation.Duration);
                youtubeVideoInstance.ElapsedTime = TimeSpan.Zero;

                result.Add(youtubeVideoInstance);
            }

            Program.Log(new LogMessage(LogSeverity.Info, "YoutubeProvider", $"Found {result.Count} videos with query {searchQuery}"));
            return result;
        }
    }
}
