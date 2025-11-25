namespace Employees.Data.Models
{
    public class EmployeeDetailResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? SupervisorId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public int DirectReportsCount { get; set; }
        public int TotalReportsCount { get; set; }
    }
}