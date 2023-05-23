using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace MikaBotRevamped
{
    class Program
    {
        public static Task Main(string[] args) => new Program().MainAsync();

        private async Task MainAsync()
        {
            var token = Environment.GetEnvironmentVariable("DISCORD_API_TOKEN", EnvironmentVariableTarget.User);

            Bot bot = new Bot(token);

            try
            {
                await bot.Start();
            } catch (Exception e)
            {
                var json = JsonConvert.SerializeObject(e, Formatting.Indented);
                await bot.Log(new LogMessage(LogSeverity.Error, "Bot", json, e));
            }
        }
    }
}