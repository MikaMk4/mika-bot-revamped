namespace MikaBotRevamped
{
    internal struct YoutubeVideoInstance
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ChannelTitle { get; set; }
        public Uri Url { get; set; }
        public Uri Thumbnail { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public MemoryStream AudioStream { get; set; }

        public YoutubeVideoInstance(string title, string description, Uri url, Uri thumbnail, TimeSpan duration, TimeSpan elapsedTime, string channelTitle)
        {
            Title = title;
            Description = description;
            Url = url;
            Thumbnail = thumbnail;
            Duration = duration;
            ElapsedTime = elapsedTime;
            ChannelTitle = channelTitle;
            AudioStream = new MemoryStream();
        }
    }
}