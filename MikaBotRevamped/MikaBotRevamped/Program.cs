using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;
using Pastel;
using System.Reflection;

namespace MikaBotRevamped
{
    class Program
    {
        public static Bot bot;

        public static Task Main(string[] args) => new Program().MainAsync();

        private async Task MainAsync()
        {
            YoutubeProvider youtubeProvider = new();

            // Dependency Injection gedöns
            DependencyProvider dependencyProvider = new();
            dependencyProvider.RegisterDependency<IYoutubeUrlProvider>(youtubeProvider);

            // Hol alle Klassen die ISlashCommand implementieren
            var slashCommandTypes = GetImplementingTypes(typeof(ISlashCommand));
            // Instanziere diese Klassen und pack sie in eine Liste
            var slashCommands = slashCommandTypes.Select(CreateInstance).Cast<ISlashCommand>().ToList();

            // Setze die Dependencies für die SlashCommands
            slashCommands.ForEach(slashCommand => slashCommand.SetDepencies(dependencyProvider));

            var token = Environment.GetEnvironmentVariable("DISCORD_API_TOKEN", EnvironmentVariableTarget.User);

            bot = new(token, slashCommands);
            try
            {
                await bot.Start();
            } catch (Exception e)
            {
                var json = JsonConvert.SerializeObject(e, Formatting.Indented);
                await Log(new LogMessage(LogSeverity.Error, "Bot", json, e));
            }
        }

        private static IEnumerable<Type> GetImplementingTypes(Type interfaceType)
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && interfaceType.IsAssignableFrom(type));
        }

        private static object CreateInstance(Type type)
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