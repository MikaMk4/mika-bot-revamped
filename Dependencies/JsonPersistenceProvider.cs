using MikaBotRevamped.Handler;
using Newtonsoft.Json;
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
        private readonly string workingDirectory;

        public JsonPersistenceProvider(string workingDirectory)
        {
            this.workingDirectory = workingDirectory;
        }

        public async Task<List<User>> LoadAllUsers()
        {
            if (!Directory.Exists(Path.Combine(workingDirectory, "Users")))
                Directory.CreateDirectory(Path.Combine(workingDirectory, "Users"));
            
            string usersDirectory = Path.Combine(workingDirectory, "Users");

            if (!Directory.Exists(usersDirectory))
                return new List<User>();

            var userList = new List<User>();
            string[] jsonFiles = Directory.GetFiles(usersDirectory, "*.json");

            foreach (string filePath in jsonFiles)
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);
                var userFromFile = JsonConvert.DeserializeObject<User>(jsonContent);
                userList.Add(userFromFile);
            }

            return userList;
        }

        public async Task SaveAllUsers(List<User> users)
        {
            users.ForEach(user => SaveSingleUser(user));
        }

        public async Task SaveSingleUser(User user)
        {
            try
            {
                string jsonContent = JsonConvert.SerializeObject(user, Formatting.Indented);
                string fileName = $"{user.Uid}.json";
                string filePath = Path.Combine(workingDirectory, "Users", fileName);

                await File.WriteAllTextAsync(filePath, jsonContent);
            }
            catch (Exception e)
            {
                Program.Log(Discord.LogSeverity.Error, "JsonPersistenceProvider", e.Message + e.StackTrace);
            }
        }
    }
}
