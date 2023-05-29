using CliWrap;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Handler
{
    public class ButtonHandler
    {
        private List<IButton> buttons = new();

        public ButtonHandler(IEnumerable<IButton> buttons)
        {
            this.buttons.AddRange(buttons);
        }

        public async Task HandleButton(SocketMessageComponent component)
        {
            if (component.User.Mention != component.Message.Interaction.User.Mention)
            {
                await component.RespondAsync("You can not use this button.", ephemeral:true);
                return;
            }

            IButton button = buttons.Find(b => b.CustomId.Equals(component.Data.CustomId));
            if (button == null)
            {
                await component.RespondAsync("Oops, something went wrong. (button == null)");
                await Program.Log(LogSeverity.Error, "ButtonHandler", "Button not found: " + component.Data.CustomId);
                return;
            }
            button.Respond(component);
        }
    }
}
