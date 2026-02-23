using Microsoft.AspNetCore.Mvc;
using SmartEmployeeSystem.Repositories;

namespace SmartEmployeeSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AdminController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("userrole")!= "Admin")
            {
                return RedirectToAction("Login","User");
            }
            return View();
        }
    }
}