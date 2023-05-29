using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliWrap;
using Discord;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using MikaBotRevamped.Handler;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace MikaBotRevamped.Dependencies
{
    internal class YoutubeProvider : IYoutubeStreamProvider, IYoutubeUrlProvider
    {
        private readonly YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer
        {
            ApiKey = Environment.GetEnvironmentVariable("YOUTUBE_API_TOKEN", EnvironmentVariableTarget.User)
        });

        public async Task<YoutubeVideoInstance> GetStreamAsync(YoutubeVideoInstance youtubeVideoInstance)
        {
            var youtube = new YoutubeClient();
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(youtubeVideoInstance.Url.ToString());
            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            var audioStream = youtube.Videos.Streams.GetAsync(streamInfo).Result;

            MemoryStream memoryStream = new MemoryStream();
            Cli.Wrap("ffmpeg")
                .WithArguments(" -hide_banner -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
                .WithStandardInputPipe(PipeSource.FromStream(audioStream))
                .WithStandardOutputPipe(PipeTarget.ToStream(memoryStream))
                .ExecuteAsync();

            youtubeVideoInstance.AudioStream = memoryStream;

            //await Task.Delay(1000);

            return youtubeVideoInstance;
        }

        public List<YoutubeVideoInstance> GeVideoInstancesFromSearch(string searchQuery, int count)
        {
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

        public YoutubeVideoInstance GetVideoInstanceFromUrl(string url)
        {
            YoutubeVideoInstance youtubeVideoInstance = new YoutubeVideoInstance();

            var videoId = GetVideoIdFromUrl(url);

            var videoRequest = youtubeService.Videos.List("snippet");
            videoRequest.Id = videoId;
            var videoResponse = videoRequest.Execute();

            if (videoResponse.Items.Count > 0)
            {
                var video = videoResponse.Items[0];

                youtubeVideoInstance.Title = video.Snippet.Title;
                youtubeVideoInstance.Description = video.Snippet.Description;
                youtubeVideoInstance.ChannelTitle = video.Snippet.ChannelTitle;
                youtubeVideoInstance.Url = new Uri($"https://www.youtube.com/watch?v={video.Id}");
                youtubeVideoInstance.Thumbnail = new Uri(video.Snippet.Thumbnails.Default__.Url);
                youtubeVideoInstance.ElapsedTime = TimeSpan.Zero;
                youtubeVideoInstance.Duration = TimeSpan.Zero;
            }
            else
            {
                Program.Log(LogSeverity.Error, "YoutubeProvider", $"Could not find video with id '{videoId}'");
                throw new Exception($"Could not find video with id '{videoId}'");
            }

            return youtubeVideoInstance;
        }

        private string GetVideoIdFromUrl(string url)
        {
            string videoId = string.Empty;

            int startIndex = url.IndexOf("v=");
            if (startIndex != -1)
            {
                startIndex += 2;  // Move past the "v=" part
                int endIndex = url.IndexOf('&', startIndex);
                if (endIndex == -1)
                {
                    endIndex = url.Length;
                }

                videoId = url.Substring(startIndex, endIndex - startIndex);
            }

            return videoId;
        }
    }
}
