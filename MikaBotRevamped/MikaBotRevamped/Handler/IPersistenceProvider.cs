using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikaBotRevamped.Handler
{
    public interface IPersistenceProvider
    {
        Task<ConcurrentDictionary<ulong, User>> LoadUsers();
        Task SaveUsers(ConcurrentDictionary<ulong, User> users);
    }
}
