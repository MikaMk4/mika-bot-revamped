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
            var loadedUsers = await persistenceProvider.LoadAllUsers();
            foreach (var user in loadedUsers)
            {
                TryAdd(user);
            }
        }

        public async Task SaveAll()
        {
            await persistenceProvider.SaveAllUsers(users.Values.ToList());
        }

        public async Task Save(ulong uid)
        {
            if (users.TryGetValue(uid, out var user))
            {
                await persistenceProvider.SaveSingleUser(user);
            }
        }

        public bool TryAdd(User user)
        {
            var added = users.TryAdd(user.Uid, user);
            if (added)
            {
                Save(user.Uid);
            }
            return added;
        }

        public bool TryRemove(ulong uid, out User removedUser)
        {
            var removed = users.TryRemove(uid, out removedUser);
            if (removed)
            {
                SaveAll();
            }
            return removed;
        }

        public User GetUser(ulong uid)
        {
            users.TryGetValue(uid, out var user);
            return user;
        }
    }
}
