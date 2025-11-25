using Employees.Data.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Employees.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _databasePath;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public EmployeeRepository(IConfiguration configuration)
        {
            _databasePath = configuration["DatabasePath"] ?? "Data/database.json";
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            var database = await LoadDatabaseAsync();
            return database.Employees.FirstOrDefault(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            var database = await LoadDatabaseAsync();
            return database.Employees;
        }

        public async Task<Employee> CreateAsync(EmployeeRequest request)
        {
            await _semaphore.WaitAsync();
            try
            {
                var database = await LoadDatabaseAsync();
                
                var newEmployee = new Employee
                {
                    Id = database.Employees.Any() ? database.Employees.Max(e => e.Id) + 1 : 1,
                    Name = request.Name,
                    Email = request.Email,
                    SupervisorId = request.SupervisorId,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                };

                database.Employees.Add(newEmployee);
                await SaveDatabaseAsync(database);

                return newEmployee;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Employee?> UpdateAsync(int id, EmployeeRequest request)
        {
            await _semaphore.WaitAsync();
            try
            {
                var database = await LoadDatabaseAsync();
                var employee = database.Employees.FirstOrDefault(e => e.Id == id);

                if (employee == null)
                    return null;

                employee.Name = request.Name;
                employee.Email = request.Email;
                employee.SupervisorId = request.SupervisorId;
                employee.LastUpdated = DateTime.UtcNow;

                await SaveDatabaseAsync(database);

                return employee;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var database = await LoadDatabaseAsync();
            return database.Employees.Any(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetDirectReportsAsync(int supervisorId)
        {
            var database = await LoadDatabaseAsync();
            return database.Employees.Where(e => e.SupervisorId == supervisorId).ToList();
        }

        public async Task<int> GetTotalReportsCountAsync(int supervisorId)
        {
            var database = await LoadDatabaseAsync();
            return await Task.Run(() => CountAllReports(supervisorId, database.Employees.ToList()));
        }

        private int CountAllReports(int supervisorId, List<Employee> allEmployees)
        {
            // Obtener empleados directos
            var directReports = allEmployees.Where(e => e.SupervisorId == supervisorId).ToList();
            
            if (!directReports.Any())
                return 0;

            // Contar empleados directos
            int count = directReports.Count;

            // Recursivamente contar empleados indirectos
            foreach (var employee in directReports)
            {
                count += CountAllReports(employee.Id, allEmployees);
            }

            return count;
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

        private async Task SaveDatabaseAsync(Database database)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(database, options);
            await File.WriteAllTextAsync(_databasePath, json);
        }
    }
}