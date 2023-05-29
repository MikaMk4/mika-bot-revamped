using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Handler
{
    public interface IButton
    {
        string CustomId { get; }
        void SetDependencies(IDependencyProvider dependencyProvider);
        Task Respond(SocketMessageComponent component);
    }
}
