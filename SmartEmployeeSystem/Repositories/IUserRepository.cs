using SmartEmployeeSystem.Models;

namespace SmartEmployeeSystem.Repositories
{
    public interface IUserRepository
    {
        void AddUser(UserModel user);
        bool IsEmailExist(string email);
        bool Login(UserModel user);
        bool VerifyPassword(string storedHash, string providedPassword);
    }
}