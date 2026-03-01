
using Microsoft.AspNetCore.Mvc;
using SmartEmployeeSystem.Repositories;

public class EmployeeController : Controller
{
    private readonly IEmployeeRepository empRepo;
    public EmployeeController(IEmployeeRepository employee)
    {
        empRepo = employee;
    }

    private bool IsEmployee()
    {
        string role = HttpContext.Session.GetString("userrole");
        return role == "Employee" || role == "HR" || role == "Admin";
    }

    public IActionResult Index()
    {
        if (!IsEmployee())
            return RedirectToAction("Login", "User");

        int userId = HttpContext.Session.GetInt32("userid") ?? 0;
        var employee = empRepo.GetEmployeeByUserId(userId);
        return View(employee);
    }
}