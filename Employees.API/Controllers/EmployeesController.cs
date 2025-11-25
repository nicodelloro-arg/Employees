using Employees.Data.Models;
using Employees.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Employees.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los empleados con su fecha de última actualización
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var employees = await _employeeService.GetAllAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employees");
                return StatusCode(500, new { message = "Error loading employees", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene los detalles completos de un empleado por ID, incluyendo empleados a cargo (directos e indirectos)
        /// La obtención se realiza en paralelo para optimizar el rendimiento
        /// </summary>
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            try
            {
                var employee = await _employeeService.GetDetailByIdAsync(id);

                if (employee == null)
                    return NotFound(new { message = $"Employee {id} not found" });

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee details {EmployeeId}", id);
                return StatusCode(500, new { message = "Error loading employee details", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un empleado por ID (versión simple)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var employee = await _employeeService.GetByIdAsync(id);

                if (employee == null)
                    return NotFound(new { message = $"Employee {id} not found" });

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee {EmployeeId}", id);
                return StatusCode(500, new { message = "Error loading employee", error = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo empleado
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(new { message = "Name is required" });

                if (string.IsNullOrWhiteSpace(request.Email))
                    return BadRequest(new { message = "Email is required" });

                var employee = await _employeeService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating employee");
                return StatusCode(500, new { message = "Error while creating employee", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un empleado existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(new { message = "Name is required" });

                if (string.IsNullOrWhiteSpace(request.Email))
                    return BadRequest(new { message = "Email is required" });

                var employee = await _employeeService.UpdateAsync(id, request);

                if (employee == null)
                    return NotFound(new { message = $"Employee {id} not found" });

                return Ok(employee);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating employee {EmployeeId}", id);
                return StatusCode(500, new { message = "Error while updating employee", error = ex.Message });
            }
        }
    }
}