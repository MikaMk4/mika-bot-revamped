using MikaBotRevamped.Handler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MikaBotRevamped.Dependencies
{
    public class JsonPersistenceProvider : IPersistenceProvider
    {
        private readonly string filePath;

        public JsonPersistenceProvider(string filePath)
        {
            this.filePath = filePath;
        }

        public async Task<ConcurrentDictionary<ulong, User>> LoadUsers()
        {
            if (!File.Exists(filePath))
                return new ConcurrentDictionary<ulong, User>();

            using var fileStream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<ConcurrentDictionary<ulong, User>>(fileStream);
        }

        public async Task SaveUsers(ConcurrentDictionary<ulong, User> users)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            using var fileStream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(fileStream, users, options);
        }
    }
}
