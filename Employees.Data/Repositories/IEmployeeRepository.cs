using Employees.Data.Models;

namespace Employees.Data.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(int id);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> CreateAsync(EmployeeRequest request);
        Task<Employee?> UpdateAsync(int id, EmployeeRequest request);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Employee>> GetDirectReportsAsync(int supervisorId);
        Task<int> GetTotalReportsCountAsync(int supervisorId);
    }
}