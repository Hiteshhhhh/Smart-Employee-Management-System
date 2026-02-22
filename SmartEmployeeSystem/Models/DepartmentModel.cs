namespace SmartEmployeeSystem.Models
{
    public class DepartmentModel
    {
        public int department_id {get;set;}
         public string? department_name { get; set; }
        public string? description { get; set; }
        public bool is_active { get; set; }
    }
}