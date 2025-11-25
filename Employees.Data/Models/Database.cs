namespace Employees.Data.Models
{
    public class Database
    {
        public List<User> Users { get; set; } = new();
        public List<Employee> Employees { get; set; } = new();
    }
}