using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    internal partial class CommandHandler
    {
        private async Task MusicCommand(SocketSlashCommand command)
        {
            var fieldName = command.Data.Options.First().Options.First().Name;

            //var value = command.Data.Options.First().Options.First().Value;

            await command.FollowupAsync(fieldName.ToString());
        }
    }
}
