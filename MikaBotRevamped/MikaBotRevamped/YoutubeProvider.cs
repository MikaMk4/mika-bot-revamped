using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeSearch;
using Discord;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

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
            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = Environment.GetEnvironmentVariable("YOUTUBE_API_TOKEN", EnvironmentVariableTarget.User)
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = searchQuery;
            searchListRequest.MaxResults = count;

            Program.Log(new LogMessage(LogSeverity.Info, "YoutubeProvider", $"Searching for {count} videos with query '{searchQuery}'"));
            var searchListResponse = searchListRequest.Execute();

            List<YoutubeVideoInstance> result = new();

            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    YoutubeVideoInstance youtubeVideoInstance = new YoutubeVideoInstance();

                    youtubeVideoInstance.Title = searchResult.Snippet.Title;

                    Program.Log(LogSeverity.Debug, "YoutubeProvider", $"Resolving data of video '{youtubeVideoInstance.Title}'");

                    youtubeVideoInstance.Description = searchResult.Snippet.Description;
                    youtubeVideoInstance.ChannelTitle = searchResult.Snippet.ChannelTitle;
                    youtubeVideoInstance.Url = new Uri($"https://www.youtube.com/watch?v={searchResult.Id.VideoId}");
                    youtubeVideoInstance.Thumbnail = new Uri(searchResult.Snippet.Thumbnails.Default__.Url);
                    youtubeVideoInstance.ElapsedTime = TimeSpan.Zero;

                    // Video duration
                    var videosListRequest = youtubeService.Videos.List("contentDetails");
                    videosListRequest.Id = searchResult.Id.VideoId;
                    var videosListResponse = videosListRequest.Execute();
                    var videoDuration = videosListResponse.Items[0].ContentDetails.Duration;
                    //var durationTimeSpan = TimeSpan.ParseExact(videoDuration, "'PT'm'M's'S'", null);

                    //youtubeVideoInstance.Duration = durationTimeSpan;

                    result.Add(youtubeVideoInstance);
                }
            }

            Program.Log(LogSeverity.Info, "YoutubeProvider", $"Found {result.Count} videos with query '{searchQuery}'");
            return result;
        }
    }
}
