using Employees.Data.Models;
using Employees.Data.Repositories;
using Employees.Service.Services;
using Moq;
using Xunit;

namespace Employees.Tests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _employeeService = new EmployeeService(_employeeRepositoryMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingEmployee_ReturnsEmployeeResponse()
        {
            // Arrange
            var employeeId = 1;
            var employee = new Employee
            {
                Id = employeeId,
                Name = "Test Employee",
                Email = "test@example.com",
                SupervisorId = null,
                CreatedDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act
            var result = await _employeeService.GetByIdAsync(employeeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(employeeId, result.Id);
            Assert.Equal(employee.Name, result.Name);
            Assert.Equal(employee.Email, result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingEmployee_ReturnsNull()
        {
            // Arrange
            var employeeId = 999;

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync((Employee?)null);

            // Act
            var result = await _employeeService.GetByIdAsync(employeeId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, Name = "Employee 1", Email = "emp1@test.com", CreatedDate = DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
                new Employee { Id = 2, Name = "Employee 2", Email = "emp2@test.com", CreatedDate = DateTime.UtcNow, LastUpdated = DateTime.UtcNow }
            };

            _employeeRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(employees);

            // Act
            var result = await _employeeService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task CreateAsync_ValidRequest_ReturnsCreatedEmployee()
        {
            // Arrange
            var request = new EmployeeRequest
            {
                Name = "New Employee",
                Email = "new@test.com",
                SupervisorId = null
            };

            var createdEmployee = new Employee
            {
                Id = 1,
                Name = request.Name,
                Email = request.Email,
                SupervisorId = request.SupervisorId,
                CreatedDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            _employeeRepositoryMock
                .Setup(x => x.CreateAsync(request))
                .ReturnsAsync(createdEmployee);

            // Act
            var result = await _employeeService.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Email, result.Email);
        }

        [Fact]
        public async Task CreateAsync_WithValidSupervisor_ReturnsCreatedEmployee()
        {
            // Arrange
            var supervisorId = 1;
            var request = new EmployeeRequest
            {
                Name = "New Employee",
                Email = "new@test.com",
                SupervisorId = supervisorId
            };

            _employeeRepositoryMock
                .Setup(x => x.ExistsAsync(supervisorId))
                .ReturnsAsync(true);

            _employeeRepositoryMock
                .Setup(x => x.CreateAsync(request))
                .ReturnsAsync(new Employee
                {
                    Id = 2,
                    Name = request.Name,
                    Email = request.Email,
                    SupervisorId = supervisorId,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                });

            // Act
            var result = await _employeeService.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(supervisorId, result.SupervisorId);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidSupervisor_ThrowsInvalidOperationException()
        {
            // Arrange
            var supervisorId = 999;
            var request = new EmployeeRequest
            {
                Name = "New Employee",
                Email = "new@test.com",
                SupervisorId = supervisorId
            };

            _employeeRepositoryMock
                .Setup(x => x.ExistsAsync(supervisorId))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _employeeService.CreateAsync(request));
        }

        [Fact]
        public async Task UpdateAsync_ValidRequest_ReturnsUpdatedEmployee()
        {
            // Arrange
            var employeeId = 1;
            var request = new EmployeeRequest
            {
                Name = "Updated Name",
                Email = "updated@test.com",
                SupervisorId = null
            };

            var updatedEmployee = new Employee
            {
                Id = employeeId,
                Name = request.Name,
                Email = request.Email,
                SupervisorId = request.SupervisorId,
                CreatedDate = DateTime.UtcNow.AddDays(-10),
                LastUpdated = DateTime.UtcNow
            };

            _employeeRepositoryMock
                .Setup(x => x.UpdateAsync(employeeId, request))
                .ReturnsAsync(updatedEmployee);

            // Act
            var result = await _employeeService.UpdateAsync(employeeId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Email, result.Email);
        }

        [Fact]
        public async Task UpdateAsync_EmployeeAsSelfSupervisor_ThrowsInvalidOperationException()
        {
            // Arrange
            var employeeId = 1;
            var request = new EmployeeRequest
            {
                Name = "Employee",
                Email = "test@test.com",
                SupervisorId = employeeId // Same as employee ID
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _employeeService.UpdateAsync(employeeId, request));
        }

        [Fact]
        public async Task GetDetailByIdAsync_ExistingEmployee_ReturnsDetailWithCounts()
        {
            // Arrange
            var employeeId = 1;
            var employee = new Employee
            {
                Id = employeeId,
                Name = "Manager",
                Email = "manager@test.com",
                SupervisorId = null,
                CreatedDate = DateTime.UtcNow.AddMonths(-6),
                LastUpdated = DateTime.UtcNow
            };

            var directReports = new List<Employee>
            {
                new Employee { Id = 2, SupervisorId = employeeId, Name = "Direct 1", Email = "d1@test.com", CreatedDate = DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
                new Employee { Id = 3, SupervisorId = employeeId, Name = "Direct 2", Email = "d2@test.com", CreatedDate = DateTime.UtcNow, LastUpdated = DateTime.UtcNow }
            };

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            _employeeRepositoryMock
                .Setup(x => x.GetDirectReportsAsync(employeeId))
                .ReturnsAsync(directReports);

            _employeeRepositoryMock
                .Setup(x => x.GetTotalReportsCountAsync(employeeId))
                .ReturnsAsync(5); // Includes indirect reports

            // Act
            var result = await _employeeService.GetDetailByIdAsync(employeeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(employeeId, result.Id);
            Assert.Equal(2, result.DirectReportsCount);
            Assert.Equal(5, result.TotalReportsCount);
        }

        [Fact]
        public async Task GetDetailByIdAsync_NonExistingEmployee_ReturnsNull()
        {
            // Arrange
            var employeeId = 999;

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync((Employee?)null);

            // Act
            var result = await _employeeService.GetDetailByIdAsync(employeeId);

            // Assert
            Assert.Null(result);
        }
    }
}