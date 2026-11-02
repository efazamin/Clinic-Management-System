using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService accountService;

        public AccountController(AccountService accountService)
        {
            this.accountService = accountService;
        }

        // LOGIN GET
        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginDTO());
        }

        // LOGIN POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginDTO login)
        {
            if (string.IsNullOrWhiteSpace(login.Email))
            {
                ModelState.AddModelError("Email", "Please enter email");
            }

            if (string.IsNullOrWhiteSpace(login.Password))
            {
                ModelState.AddModelError("Password", "Please enter password");
            }

            if (ModelState.IsValid)
            {
                var user = accountService.Login(login);

                if (user != null)
                {
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("UserName", user.FullName);
                    HttpContext.Session.SetString("UserEmail", user.Email);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid email or password");
            }

            return View(login);
        }

        // REGISTER GET
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterDTO());
        }

        // REGISTER POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterDTO register)
        {
            if (string.IsNullOrWhiteSpace(register.FullName))
            {
                ModelState.AddModelError("FullName", "Please enter full name");
            }

            if (string.IsNullOrWhiteSpace(register.Email))
            {
                ModelState.AddModelError("Email", "Please enter email");
            }

            if (string.IsNullOrWhiteSpace(register.Password))
            {
                ModelState.AddModelError("Password", "Please enter password");
            }

            if (string.IsNullOrWhiteSpace(register.ConfirmPassword))
            {
                ModelState.AddModelError("ConfirmPassword", "Please confirm password");
            }

            if (register.Password != register.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Password and confirm password do not match");
            }

            if (!string.IsNullOrWhiteSpace(register.Email) && accountService.EmailExists(register.Email))
            {
                ModelState.AddModelError("Email", "This email already exists");
            }

            if (ModelState.IsValid)
            {
                var result = accountService.Register(register);

                if (result)
                {
                    TempData["SuccessMessage"] = "Account created successfully. Please sign in.";
                    return RedirectToAction("Login");
                }

                TempData["ErrorMessage"] = "Account create failed.";
            }

            return View(register);
        }

        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}