namespace SmartEmployeeSystem.Models
{
    public class UserModel
    {
        public int user_id { get; set; }
        public string? username { get; set; }
        public string? password_hash { get; set; }
        public string? email { get; set; }
        public string? role { get; set; }
        public bool is_active { get; set; }
    }
}