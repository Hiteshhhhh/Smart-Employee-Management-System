using SmartEmployeeSystem.Models;

namespace SmartEmployeeSystem.Repositories
{
    public interface IDepartmentRepository
    {
        List<DepartmentModel> GetDepartments();
        DepartmentModel GetDepartmentById(int id);
        void AddDepartment(DepartmentModel department);
        void UpdateDepartment(DepartmentModel department);
        void DeleteDepartment(int id);
    }
}