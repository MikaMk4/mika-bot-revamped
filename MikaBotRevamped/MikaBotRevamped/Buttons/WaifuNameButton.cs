using CliWrap;
using Discord;
using Discord.WebSocket;
using MikaBotRevamped.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Buttons
{
    internal class WaifuNameButton : IButton
    {
        public string CustomId => "give-waifu-name-button";

        public async Task Respond(SocketMessageComponent component)
        {
            ModalBuilder modalBuilder = new ModalBuilder();
            modalBuilder.Title = "Give your Waifu a name!";
            modalBuilder.WithCustomId("waifu-name-input");
            modalBuilder.AddTextInput("Name", "waifu-name", minLength: 1, maxLength: 32);
            await component.RespondWithModalAsync(modalBuilder.Build());
        }

        public void SetDependencies(IDependencyProvider dependencyProvider)
        {
            return;
        }
    }
}
