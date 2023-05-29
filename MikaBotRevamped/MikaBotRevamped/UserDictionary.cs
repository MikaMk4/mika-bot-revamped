using MikaBotRevamped.Handler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped
{
    public class UserDictionary
    {
        private readonly ConcurrentDictionary<ulong, User> users;
        private readonly IPersistenceProvider persistenceProvider;

        public ConcurrentDictionary<ulong, User> Users => users;

        public UserDictionary(IPersistenceProvider persistenceProvider)
        {
            this.persistenceProvider = persistenceProvider;
            users = new ConcurrentDictionary<ulong, User>();
        }

        public async Task Load()
        {
            var loadedUsers = await persistenceProvider.LoadUsers();
            foreach (var user in loadedUsers)
            {
                users.TryAdd(user.Key, user.Value);
            }
        }

        public async Task Save()
        {
            await persistenceProvider.SaveUsers(users);
        }

        public bool TryAdd(ulong uid, User user)
        {
            var added = users.TryAdd(uid, user);
            if (added)
            {
                Save();
            }
            return added;
        }

        public bool TryRemove(ulong uid, out User removedUser)
        {
            var removed = users.TryRemove(uid, out removedUser);
            if (removed)
            {
                Save();
            }
            return removed;
        }
    }
}
