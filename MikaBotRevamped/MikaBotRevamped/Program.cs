using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MikaBotRevamped.Dependencies;
using MikaBotRevamped.Handler;
using Newtonsoft.Json;
using Pastel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MikaBotRevamped
{
    class Program
    {
        public static Bot bot;

        public static readonly ulong MikaUid = 452415473687068672;

        private static DependencyProvider dependencyProvider = new();

        public static Task Main(string[] args) => new Program().MainAsync();

        private async Task MainAsync()
        {
            YoutubeProvider youtubeProvider = new();
            WaifuProvider waifuProvider = new("https://api.waifu.pics/", "https://api.waifu.im/");
            GachaProvider gachaProvider = new();

            // Dependency Injection gedöns
            dependencyProvider.RegisterDependency<IYoutubeUrlProvider>(youtubeProvider);
            dependencyProvider.RegisterDependency<IYoutubeStreamProvider>(youtubeProvider);
            dependencyProvider.RegisterDependency<IWaifuProvider>(waifuProvider);
            dependencyProvider.RegisterDependency<IGachaProvider>(gachaProvider);
            dependencyProvider.RegisterDependency<IDependencyProvider>(dependencyProvider);

            // Hol alle Klassen die IButton implementieren
            var buttonTypes = GetImplementingTypes(typeof(IButton));
            // Instanziere diese Klassen und pack sie in eine Liste
            var buttons = buttonTypes.Select(CreateInstance).Cast<IButton>().ToList();

            // Setze die Dependencies für die IButtons
            buttons.ForEach(button => button.SetDependencies(dependencyProvider));

            var token = Environment.GetEnvironmentVariable("DISCORD_API_TOKEN", EnvironmentVariableTarget.User);

            bot = new(token, buttons);
            try
            {
                await bot.Start();
            } catch (Exception e)
            {
                var json = JsonConvert.SerializeObject(e, Formatting.Indented);
                await Log(new LogMessage(LogSeverity.Error, "Bot", json, e));
            }
        }

        public async static Task RegisterAllSlashCommands()
        {
            Log(LogSeverity.Info, "Program", "Registering all slash commands");

            bot.SlashCommandHandler.ClearSlashCommands();

            // Hol alle Klassen die ISlashCommand implementieren
            var slashCommandTypes = GetImplementingTypes(typeof(ISlashCommand));
            // Instanziere diese Klassen und pack sie in eine Liste
            var slashCommands = slashCommandTypes.Select(CreateInstance).Cast<ISlashCommand>().ToList();

            // Setze die Dependencies für die SlashCommands und Buttons
            slashCommands.ForEach(slashCommand => slashCommand.SetDependencies(dependencyProvider));

            foreach (var slashCommand in slashCommands)
            {
                bot.SlashCommandHandler.AddSlashCommand(slashCommand);
            }

            await bot.SlashCommandHandler.RegisterCommands();
        }

        public static IEnumerable<Type> GetImplementingTypes(Type interfaceType)
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && interfaceType.IsAssignableFrom(type));
        }

        public static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public static Task Log(LogMessage msg)
        {
            const int severityAlign = -10;
            const int sourceAlign = -15;

            string text = $"{DateTime.Now:HH:mm:ss} {msg.Source,sourceAlign} {msg.Message}";

            if (msg.Exception != null)
            {
                text += $"\n{msg.Exception}";
            }

            switch (msg.Severity)
            {
                case LogSeverity.Error:
                    text = $"{"[ERROR]",severityAlign} {text}".Pastel(Color.Red);
                    break;

                case LogSeverity.Warning:
                    text = $"{"[WARNING]",severityAlign} {text}".Pastel(Color.Orange);
                    break;

                case LogSeverity.Debug:
                    text = $"{"[DEBUG]",severityAlign} {text}".Pastel(Color.LighterGrey);
                    break;

                case LogSeverity.Verbose:
                    text = $"{"[VERBOSE]",severityAlign} {text}";
                    break;

                case LogSeverity.Critical:
                    text = $"{"[CRITICAL]",severityAlign} {text}".Pastel(Color.Red);
                    break;

                case LogSeverity.Info:
                    text = $"{"[INFO]",severityAlign} {text}";
                    break;
            }

            Console.WriteLine(text);
            return Task.CompletedTask;
        }

        public static Task Log(LogSeverity logSeverity, string source, string message, Exception exception = null)
        {
            Log(new LogMessage(logSeverity, source, message, exception));
            return Task.CompletedTask;
        }
    }

    public static class EnumExtension
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
           => self.Select((item, index) => (item, index));
    }
}