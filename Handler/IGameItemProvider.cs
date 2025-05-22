using MikaBotRevamped.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Handler
{
    internal interface IGameItemProvider
    {
        IGameItem GetGameItem(int id);
        int GetGameItemId(string name);
    }
}
