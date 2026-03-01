namespace SmartEmployeeSystem.Models
{
    public class EmployeeModel
    {
        public int employee_id { get; set; }
        public int user_id { get; set; }
        public string? username { get; set; }
        public string? email { get; set; }
        public int department_id { get; set; }
        public string? employee_code { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? designation { get; set; }
        public decimal base_salary { get; set; }
        public DateTime date_of_joining { get; set; }
        public string? department_name { get; set; }
        public IFormFile? Image { get; set; }
    }
}