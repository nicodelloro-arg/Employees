using Employees.Data.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Employees.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _databasePath;

        public UserRepository(IConfiguration configuration)
        {
            _databasePath = configuration["DatabasePath"] ?? "Data/database.json";
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var database = await LoadDatabaseAsync();
            return database.Users.FirstOrDefault(u => 
                u.Username == username && u.Password == password);
        }

        private async Task<Database> LoadDatabaseAsync()
        {
            if (!File.Exists(_databasePath))
            {
                throw new FileNotFoundException($"Database file not found at: {Path.GetFullPath(_databasePath)}");
            }

            var json = await File.ReadAllTextAsync(_databasePath);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            return JsonSerializer.Deserialize<Database>(json, options) ?? new Database();
        }
    }
}