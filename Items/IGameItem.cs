using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Items
{
    public interface IGameItem
    {
        int Id { get; }
        string Name { get; }
        string Description { get; }
        Emoji Emoji { get; }
    }
}
