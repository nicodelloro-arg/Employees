using Employees.Data.Models;

namespace Employees.Service.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}