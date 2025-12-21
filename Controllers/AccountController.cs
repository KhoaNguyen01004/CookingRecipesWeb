using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Supabase;
using CookingRecipesWeb.Services;
using CookingRecipesWeb.Models;

namespace CookingRecipesWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly Client _client;
        private readonly UserService _userService;

        public AccountController(Client client, UserService userService)
        {
            _client = client;
            _userService = userService;
        }

        // ======================
        // LOGIN (GET)
        // ======================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ======================
        // LOGIN (POST)
        // ======================
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var session = await _client.Auth.SignIn(email, password);

                if (session?.User == null)
                {
                    TempData["ErrorMessage"] = "Invalid email or password.";
                    return RedirectToAction("Login");
                }

                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User profile not found.";
                    return RedirectToAction("Login");
                }

                HttpContext.Session.SetString("UserId", session.User.Id);
                HttpContext.Session.SetString("UserEmail", email);
                HttpContext.Session.SetString("FirstName", user.FirstName);

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                TempData["ErrorMessage"] = "Login failed.";
                return RedirectToAction("Login");
            }
        }

        // ======================
        // REGISTER (GET)
        // ======================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ======================
        // REGISTER (POST)
        // ======================
        [HttpPost]
        public async Task<IActionResult> Register(
            string firstName,
            string lastName,
            string email,
            string password,
            string confirmPassword)
        {
            if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Passwords do not match.";
                return RedirectToAction("Register");
            }

            var session = await _client.Auth.SignUp(email, password);

            if (session?.User == null)
            {
                TempData["ErrorMessage"] = "Registration failed.";
                return RedirectToAction("Register");
            }

            await _userService.CreateUserAsync(new User
            {
                Id = Guid.Parse(session.User.Id),
                FirstName = firstName,
                LastName = lastName,
                Email = email
            });

            HttpContext.Session.SetString("UserId", session.User.Id);
            HttpContext.Session.SetString("UserEmail", email);
            HttpContext.Session.SetString("FirstName", firstName);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _client.Auth.SignOut();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
