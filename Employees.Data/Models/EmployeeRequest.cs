namespace Employees.Data.Models
{
    public class EmployeeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? SupervisorId { get; set; }
    }
}