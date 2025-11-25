using Employees.Data.Models;

namespace Employees.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User?> ValidateUserAsync(string username, string password);
    }
}