using Microsoft.AspNetCore.Mvc;
using SmartEmployeeSystem.Models;
using SmartEmployeeSystem.Repositories;

namespace SmartEmployeeSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            if (_userRepository.IsEmailExist(user.email))
            {
                ViewBag.msg = "Email already registered!";
                return View();
            }
            _userRepository.AddUser(user);
            TempData["success"] = "Registered successfully! Please login.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            if (_userRepository.Login(user))
            {
                string role = HttpContext.Session.GetString("userrole");

                if (role == "Admin")
                    return RedirectToAction("Index", "Admin");
                else if (role == "HR")
                    return RedirectToAction("Index", "HR");
                else
                    return RedirectToAction("Index", "Employee");
            }
            TempData["error"] = "Invalid email or password!";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}