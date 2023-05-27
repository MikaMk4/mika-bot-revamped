using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    internal interface ISlashCommand
    {
        string CommandName { get; }
        bool AsyncMode { get; }
        void SetDepencies(IDependencyProvider dependencyProvider);
        Task Respond(SocketSlashCommand command);
        SlashCommandProperties GetCommandProperties();
    }
}
