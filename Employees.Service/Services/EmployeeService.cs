using Employees.Data.Models;
using Employees.Data.Repositories;

namespace Employees.Service.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<EmployeeResponse?> GetByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            return employee == null ? null : MapToResponse(employee);
        }

        public async Task<EmployeeDetailResponse?> GetDetailByIdAsync(int id)
        {
            // Ejecutar en paralelo: obtener empleado y contar reportes
            var employeeTask = _employeeRepository.GetByIdAsync(id);
            var directReportsTask = _employeeRepository.GetDirectReportsAsync(id);
            var totalReportsTask = _employeeRepository.GetTotalReportsCountAsync(id);

            // Esperar a que todas las tareas se completen
            await Task.WhenAll(employeeTask, directReportsTask, totalReportsTask);

            var employee = await employeeTask;
            if (employee == null)
                return null;

            var directReports = await directReportsTask;
            var totalReports = await totalReportsTask;

            return new EmployeeDetailResponse
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                SupervisorId = employee.SupervisorId,
                CreatedDate = employee.CreatedDate,
                LastUpdated = employee.LastUpdated,
                DirectReportsCount = directReports.Count(),
                TotalReportsCount = totalReports
            };
        }

        public async Task<IEnumerable<EmployeeResponse>> GetAllAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Select(MapToResponse);
        }

        public async Task<EmployeeResponse> CreateAsync(EmployeeRequest request)
        {
            // Validar que el supervisor existe si se proporciona
            if (request.SupervisorId.HasValue)
            {
                var supervisorExists = await _employeeRepository.ExistsAsync(request.SupervisorId.Value);
                if (!supervisorExists)
                    throw new InvalidOperationException($"Supervisor ID {request.SupervisorId} does not exist");
            }

            var employee = await _employeeRepository.CreateAsync(request);
            return MapToResponse(employee);
        }

        public async Task<EmployeeResponse?> UpdateAsync(int id, EmployeeRequest request)
        {
            // Validar que el supervisor existe si se proporciona
            if (request.SupervisorId.HasValue)
            {
                // No puede ser supervisor de sí mismo
                if (request.SupervisorId.Value == id)
                    throw new InvalidOperationException("Employee cannot be its own supervisor");

                var supervisorExists = await _employeeRepository.ExistsAsync(request.SupervisorId.Value);
                if (!supervisorExists)
                    throw new InvalidOperationException($"Supervisor ID {request.SupervisorId} does not exist");
            }

            var employee = await _employeeRepository.UpdateAsync(id, request);
            return employee == null ? null : MapToResponse(employee);
        }

        private static EmployeeResponse MapToResponse(Employee employee)
        {
            return new EmployeeResponse
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                SupervisorId = employee.SupervisorId,
                CreatedDate = employee.CreatedDate,
                LastUpdated = employee.LastUpdated
            };
        }
    }
}