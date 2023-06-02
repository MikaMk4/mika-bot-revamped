using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Handler
{
    internal interface IGachaProvider
    {
        //UnclaimedWaifu Roll(IWaifuProvider waifuProvider, int pityCount);
        Embed RollAndBuildEmbed(IWaifuProvider waifuProvider, IGameItemProvider gameItemProvider, SocketSlashCommand command);
    }
}
