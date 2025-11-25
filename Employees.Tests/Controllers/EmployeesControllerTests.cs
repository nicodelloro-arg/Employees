using Employees.API.Controllers;
using Employees.Data.Models;
using Employees.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Employees.Tests.Controllers
{
    public class EmployeesControllerTests
    {
        private readonly Mock<IEmployeeService> _employeeServiceMock;
        private readonly Mock<ILogger<EmployeesController>> _loggerMock;
        private readonly EmployeesController _controller;

        public EmployeesControllerTests()
        {
            _employeeServiceMock = new Mock<IEmployeeService>();
            _loggerMock = new Mock<ILogger<EmployeesController>>();
            _controller = new EmployeesController(_employeeServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithEmployees()
        {
            // Arrange
            var employees = new List<EmployeeResponse>
            {
                new EmployeeResponse { Id = 1, Name = "Employee 1", Email = "emp1@test.com" },
                new EmployeeResponse { Id = 2, Name = "Employee 2", Email = "emp2@test.com" }
            };

            _employeeServiceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(employees);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEmployees = Assert.IsAssignableFrom<IEnumerable<EmployeeResponse>>(okResult.Value);
            Assert.Equal(2, returnedEmployees.Count());
        }

        [Fact]
        public async Task GetById_ExistingEmployee_ReturnsOk()
        {
            // Arrange
            var employeeId = 1;
            var employee = new EmployeeResponse
            {
                Id = employeeId,
                Name = "Test Employee",
                Email = "test@test.com"
            };

            _employeeServiceMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act
            var result = await _controller.GetById(employeeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEmployee = Assert.IsType<EmployeeResponse>(okResult.Value);
            Assert.Equal(employeeId, returnedEmployee.Id);
        }

        [Fact]
        public async Task GetById_NonExistingEmployee_ReturnsNotFound()
        {
            // Arrange
            var employeeId = 999;

            _employeeServiceMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync((EmployeeResponse?)null);

            // Act
            var result = await _controller.GetById(employeeId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetDetails_ExistingEmployee_ReturnsOkWithDetails()
        {
            // Arrange
            var employeeId = 1;
            var employeeDetails = new EmployeeDetailResponse
            {
                Id = employeeId,
                Name = "Manager",
                Email = "manager@test.com",
                DirectReportsCount = 2,
                TotalReportsCount = 5
            };

            _employeeServiceMock
                .Setup(x => x.GetDetailByIdAsync(employeeId))
                .ReturnsAsync(employeeDetails);

            // Act
            var result = await _controller.GetDetails(employeeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDetails = Assert.IsType<EmployeeDetailResponse>(okResult.Value);
            Assert.Equal(2, returnedDetails.DirectReportsCount);
            Assert.Equal(5, returnedDetails.TotalReportsCount);
        }

        [Fact]
        public async Task Create_ValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var request = new EmployeeRequest
            {
                Name = "New Employee",
                Email = "new@test.com",
                SupervisorId = null
            };

            var createdEmployee = new EmployeeResponse
            {
                Id = 1,
                Name = request.Name,
                Email = request.Email
            };

            _employeeServiceMock
                .Setup(x => x.CreateAsync(request))
                .ReturnsAsync(createdEmployee);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedEmployee = Assert.IsType<EmployeeResponse>(createdResult.Value);
            Assert.Equal(request.Name, returnedEmployee.Name);
        }

        [Fact]
        public async Task Create_EmptyName_ReturnsBadRequest()
        {
            // Arrange
            var request = new EmployeeRequest
            {
                Name = "",
                Email = "test@test.com"
            };

            // Act
            var result = await _controller.Create(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_EmptyEmail_ReturnsBadRequest()
        {
            // Arrange
            var request = new EmployeeRequest
            {
                Name = "Test",
                Email = ""
            };

            // Act
            var result = await _controller.Create(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ValidRequest_ReturnsOk()
        {
            // Arrange
            var employeeId = 1;
            var request = new EmployeeRequest
            {
                Name = "Updated Name",
                Email = "updated@test.com"
            };

            var updatedEmployee = new EmployeeResponse
            {
                Id = employeeId,
                Name = request.Name,
                Email = request.Email
            };

            _employeeServiceMock
                .Setup(x => x.UpdateAsync(employeeId, request))
                .ReturnsAsync(updatedEmployee);

            // Act
            var result = await _controller.Update(employeeId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEmployee = Assert.IsType<EmployeeResponse>(okResult.Value);
            Assert.Equal(request.Name, returnedEmployee.Name);
        }

        [Fact]
        public async Task Update_NonExistingEmployee_ReturnsNotFound()
        {
            // Arrange
            var employeeId = 999;
            var request = new EmployeeRequest
            {
                Name = "Updated Name",
                Email = "updated@test.com"
            };

            _employeeServiceMock
                .Setup(x => x.UpdateAsync(employeeId, request))
                .ReturnsAsync((EmployeeResponse?)null);

            // Act
            var result = await _controller.Update(employeeId, request);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}