using Employees.Data.Models;

namespace Employees.Service.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeResponse?> GetByIdAsync(int id);
        Task<EmployeeDetailResponse?> GetDetailByIdAsync(int id);
        Task<IEnumerable<EmployeeResponse>> GetAllAsync();
        Task<EmployeeResponse> CreateAsync(EmployeeRequest request);
        Task<EmployeeResponse?> UpdateAsync(int id, EmployeeRequest request);
    }
}