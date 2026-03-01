
using Microsoft.AspNetCore.Mvc;
using SmartEmployeeSystem.Repositories;
using SmartEmployeeSystem.Models;

namespace SmartEmployeeSystem.Controllers
{
    public class HRController : Controller
    {
        private readonly IEmployeeRepository employee;
        private readonly IDepartmentRepository department;
        public HRController(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            employee = employeeRepository;
            department = departmentRepository;
        }

        private bool IsHR()
        {
            string role = HttpContext.Session.GetString("userrole");
            return role == "HR" || role == "Admin";
        }

        public IActionResult Index()
        {
            if (!IsHR())
            {
                return RedirectToAction("Login", "User");
            }
            var employees = employee.GetEmployees();
            return View(employees);
        }

        [HttpGet]
        public IActionResult AddEmployee()
        {
            if (!IsHR())
                return RedirectToAction("Login", "User");

            // Department dropdown ke liye
            ViewBag.Departments = department.GetDepartments();
            return View();
        }
        [HttpPost]
        public IActionResult AddEmployee(EmployeeModel employees)
        {
            employee.AddEmployee(employees);
            TempData["success"] = "Employee added successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            if (!IsHR())
                return RedirectToAction("Login", "User");

            var employees = employee.GetEmployeeModelById(id);
            ViewBag.Departments = department.GetDepartments();
            return View(employees);
        }

        [HttpPost]
        public IActionResult EditEmployee(EmployeeModel employees)
        {
            employee.UpdateEmployee(employees);
            TempData["success"] = "Employee updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteEmployee(int id)
        {
            if (!IsHR())
                return RedirectToAction("Login", "User");

            employee.DeleteEmployee(id);
            TempData["success"] = "Employee deleted successfully!";
            return RedirectToAction("Index");
        }

    }
}