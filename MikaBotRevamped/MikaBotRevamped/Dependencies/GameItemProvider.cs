using MikaBotRevamped.Handler;
using MikaBotRevamped.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Dependencies
{
    internal class GameItemProvider : IGameItemProvider
    {
        private readonly Dictionary<int, IGameItem> gameItems = new Dictionary<int, IGameItem>();

        public GameItemProvider()
        {
            var itemTypes = Program.GetImplementingTypes(typeof(IGameItem));
            var items = itemTypes.Select(Program.CreateInstance).Cast<IGameItem>().ToList();

            items.ForEach(item => { gameItems.Add(item.Id, item); });
        }

        public IGameItem GetGameItem(int id) => gameItems[id];

        public int GetGameItemId(string name)
        {
            var item = gameItems.Values.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return item?.Id ?? 0;
        }
    }
}
