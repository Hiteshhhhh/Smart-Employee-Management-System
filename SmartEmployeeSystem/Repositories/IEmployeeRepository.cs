using SmartEmployeeSystem.Models;
namespace SmartEmployeeSystem.Repositories
{
    public interface IEmployeeRepository
    {
        List<EmployeeModel> GetEmployees();
        EmployeeModel GetEmployeeModelById(int id);
        EmployeeModel GetEmployeeByUserId(int userId);
        void AddEmployee(EmployeeModel employee);
        void UpdateEmployee(EmployeeModel employee);
        void DeleteEmployee(int id);
    }
}