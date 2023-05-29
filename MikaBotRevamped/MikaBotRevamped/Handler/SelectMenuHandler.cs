using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Handler
{
    internal partial class SelectMenuHandler
    {
        private readonly DiscordSocketClient client;

        public SelectMenuHandler(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task ResolveSelectMenu(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "searchResultSelectMenu":
                    await SearchResultSelectMenu(component);
                    break;
                default:
                    await component.RespondAsync("Oops, something went wrong. (default case)");
                    break;
            }
        }

        private async Task SearchResultSelectMenu(SocketMessageComponent component)
        {
            await component.RespondAsync(component.Data.Values.First());

            if (Program.bot.guilds[(ulong)component.GuildId].Users.Any(user => user.Id == component.User.Id))
            {
                var message = Program.bot.guilds[(ulong)component.GuildId].Users.Find(user => user.Id == component.User.Id).FollowupMessage;
                await message.DeleteAsync();

                EmbedBuilder embedBuilder = new();
                embedBuilder.WithTitle("Chosen Song");
                embedBuilder.WithDescription("You have chosen the song: " + component.Data.Values.First());
                embedBuilder.WithColor(Color.Green);
            }
        }
    }
}
