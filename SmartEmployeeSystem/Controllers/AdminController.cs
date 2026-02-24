using Microsoft.AspNetCore.Mvc;
using SmartEmployeeSystem.Repositories;
using SmartEmployeeSystem.Models;

namespace SmartEmployeeSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _department;

        public AdminController(IUserRepository userRepository, IDepartmentRepository departmentRepository)
        {
            _userRepository = userRepository;
            _department = departmentRepository;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("userrole") == "Admin";
        }

        public IActionResult Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }
        public IActionResult Departments()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");
            var departments = _department.GetDepartments();
            return View(departments);
        }

        [HttpGet]
        public IActionResult AddDepartment()
        {
            if (!IsAdmin()) 
                return RedirectToAction("Login", "User");
            return View();
        }

        [HttpPost]
        public IActionResult AddDepartment(DepartmentModel department)
        {
            _department.AddDepartment(department);
            TempData["success"] = "Department added successfully!";
            return RedirectToAction("Departments");
        }
        
        [HttpGet]
        public IActionResult EditDepartment(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
            }
            var department = _department.GetDepartmentById(id);
            return View(department);
        }

        [HttpPost]
        public IActionResult EditDepartment(DepartmentModel department)
        {
            _department.UpdateDepartment(department);
            TempData["success"] = "Department updated successfully!";
            return RedirectToAction("Departments");
        }

        [HttpPost]
        public IActionResult DeleteDepartment(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "User");
            }
            _department.DeleteDepartment(id);
            TempData["success"] = "Department deleted successfully!";
            return RedirectToAction("Departments");
        }
    }
}