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
            }
        }

        public void SetDependencies(IDependencyProvider dependencyProvider)
        {
            dependencyProvider = dependencyProvider.GetDependency<IDependencyProvider>();
        }
    }
}
