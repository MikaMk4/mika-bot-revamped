using Discord;
using Discord.WebSocket;
using MikaBotRevamped.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Commands
{
    internal class DebugCommand : ISlashCommand
    {
        public string CommandName => "debug";

        public bool AsyncMode => false;

        private IDependencyProvider dependencyProvider;

        public SlashCommandProperties GetCommandProperties()
        {
            return new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Debug command")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("register-all-commands")
                    .WithDescription("Register all commands")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("register-single-command")
                    .WithDescription("Register a single command")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("name")
                        .WithDescription("Name of the command")
                        .WithType(ApplicationCommandOptionType.String)
                        .WithRequired(true)
                    )
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("register-single-command-globally")
                    .WithDescription("Register a single command globally")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("name")
                        .WithDescription("Name of the command")
                        .WithType(ApplicationCommandOptionType.String)
                        .WithRequired(true)
                    )
                )
                .Build();
        }

        public async Task Respond(SocketSlashCommand command)
        {
            if (command.User.Id != Program.MikaUid){
                await command.RespondAsync("You are not allowed to use this command.", ephemeral:true);
                return;
            }

            var subCommand = command.Data.Options.First();

            if (subCommand.Name == "register-all-commands")
            {
                await command.DeferAsync(ephemeral:true);
                await Program.RegisterAllSlashCommands();
                await command.FollowupAsync("Registered all commands.", ephemeral:true);
            } else if (subCommand.Name == "register-single-command")
            {
                var name = subCommand.Options.First().Value.ToString();
                var commandType = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ISlashCommand))).FirstOrDefault(t => t.Name.ToLower() == name.ToLower() + "command");
                if (commandType == null)
                {
                    await command.RespondAsync("Command not found.", ephemeral:true);
                    return;
                }
                var commandInstance = (ISlashCommand)Activator.CreateInstance(commandType);
                commandInstance.SetDependencies(dependencyProvider);
                await command.DeferAsync(ephemeral:true);
                await Program.RegisterSlashCommand(commandInstance);
                await command.FollowupAsync($"Registered command {name}.", ephemeral:true);
            } else if (subCommand.Name == "register-single-command-globally")
            {
                var name = subCommand.Options.First().Value.ToString();
                var commandType = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ISlashCommand))).FirstOrDefault(t => t.Name.ToLower() == name.ToLower() + "command");
                if (commandType == null)
                {
                    await command.RespondAsync("Command not found.", ephemeral:true);
                    return;
                }
                var commandInstance = (ISlashCommand)Activator.CreateInstance(commandType);
                commandInstance.SetDependencies(dependencyProvider);
                await command.DeferAsync(ephemeral:true);
                await Program.bot.SlashCommandHandler.RegisterCommand(commandInstance);
                await command.FollowupAsync($"Registered command {name} globally.", ephemeral:true);
            }
        }

        public void SetDependencies(IDependencyProvider dependencyProvider)
        {
            this.dependencyProvider = dependencyProvider.GetDependency<IDependencyProvider>();
        }
    }
}
