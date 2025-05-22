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
        Task<List<User>> LoadAllUsers();
        Task SaveAllUsers(List<User> users);
        Task SaveSingleUser(User user);
    }
}
