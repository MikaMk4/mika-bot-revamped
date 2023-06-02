using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using MikaBotRevamped.Items;

namespace MikaBotRevamped
{
    [Serializable]
    public class User
    {
        public ulong Uid { get; }
        public int PityCount { get; set; } = 0;
        public List<Waifu> Waifus { get; set; } = new List<Waifu>();
        public List<UnclaimedWaifu> UnclaimedWaifus { get; set; } = new List<UnclaimedWaifu>();
        public List<StackableItem> Items { get; set; } = new List<StackableItem>();
        public DateTime LastDaily { get; set; } = DateTime.MinValue;

        [NonSerialized]
        public RestFollowupMessage? RestFollowupMessage;

        [JsonConstructor]
        public User(ulong uid)
        {
            Uid = uid;
        }

        public void AddItem(IGameItem item, int amount)
        {
            var stackableItem = Items.FirstOrDefault(x => x.Id == item.Id);
            if (stackableItem.Equals(default(StackableItem)))
            {
                Items.Add(new StackableItem(item.Id, amount));
            }
            else
            {
                Items.Remove(stackableItem);
                stackableItem.Amount += amount;
                Items.Add(stackableItem);
            }
        }

        public int GetItemCount(IGameItem item)
        {
            var stackableItem = Items.FirstOrDefault(x => x.Id == item.Id);
            if (stackableItem.Equals(default(StackableItem)))
            {
                return 0;
            }
            else
            {
                return stackableItem.Amount;
            }
        }

        public void RemoveItem(IGameItem item, int amount)
        {
            var stackableItem = Items.FirstOrDefault(x => x.Id == item.Id);
            if (stackableItem.Equals(default(StackableItem)))
            {
                return;
            }
            else
            {
                if (stackableItem.Amount < amount)
                {
                    throw new InvalidOperationException($"Wanted to remove {amount} of item id {stackableItem.Id} but only had {stackableItem.Amount}.");
                }

                Items.Remove(stackableItem);
                stackableItem.Amount -= amount;
                if (stackableItem.Amount > 0)
                {
                    Items.Add(stackableItem);
                }
            }
        }
    }
}
