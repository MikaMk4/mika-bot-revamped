using Discord;
using Discord.WebSocket;
using MikaBotRevamped.Handler;
using MikaBotRevamped.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Commands
{
    internal class DailyCommand : ISlashCommand
    {
        public string CommandName => "daily";

        public bool AsyncMode => false;

        public SlashCommandProperties GetCommandProperties()
        {
            return new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Claim your daily reward!")
                .Build();
        }

        public async Task Respond(SocketSlashCommand command)
        {
            var user = Program.bot.Users.GetUser(command.User.Id);

            if (user == null)
            {
                await command.RespondAsync("You are not registered! First roll a Waifu to register.", ephemeral:true);
                return;
            }

            var timeElapsed = DateTime.Now - user.LastDaily;
            if (timeElapsed < TimeSpan.FromHours(24))
            {
                await command.RespondAsync($"You have already claimed your daily reward today! ({TimeSpan.FromHours(24) - timeElapsed:hh\\:mm\\:ss} left)", ephemeral:true);
                return;
            }

            user.LastDaily = DateTime.Now;
            user.AddItem(new Roll(), 10);
            Program.bot.Users.Save(user.Uid);

            await command.RespondAsync("You have claimed your daily reward! You received 10 rolls.");
            return;
        }

        public void SetDependencies(IDependencyProvider dependencyProvider)
        {
            return;
        }
    }
}
