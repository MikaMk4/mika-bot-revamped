using CommandLine;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace MikaBotRevamped
{
    class Program
    {
        String token = Environment.GetEnvironmentVariable("DISCORD_API_TOKEN", EnvironmentVariableTarget.User);

        const ulong MondstadtGuildId = 870773459104436245;

        [Verb("register", HelpText = "Register commands.")]
        public class RegisterOptions
        {
            [Option(longName: "guild", SetName= "locality", HelpText = "Register all guild commands of a guild with given guild-id")]
            public bool AllGuildCommands { get; set; }

            [Option(longName: "global", SetName = "locality", HelpText = "Register all global commands.")]
            public bool AllGlobalCommands { get; set; }
        }

        [Verb("run", HelpText = "Run the bot")]
        public class RunOptions
        {
            [Option(longName: "run", Required = false, HelpText = "Run the bot.")]
            public bool Run { get; set; }
        }

        public static Task Main(string[] args) => new Program().MainAsync(args);

        private async Task MainAsync(string[] args)
        {
            Parser.Default.ParseArguments<RegisterOptions, RunOptions>(args)
                .WithParsed<RegisterOptions>(async o => await RegisterCommands())
                .WithParsed<RunOptions>(async o => await RunBot());
        }

        private async Task RegisterCommands()
        {
            Bot bot = new Bot(token);

            await bot.RegisterCommands(MondstadtGuildId);
        }

        private async Task RunBot()
        {
            Bot bot = new Bot(token);

            try
            {
                await bot.Start();
            }
            catch (Exception e)
            {
                var json = JsonConvert.SerializeObject(e, Formatting.Indented);
                await bot.Log(new LogMessage(LogSeverity.Error, "Bot", json, e));
            }
        }
    }
}