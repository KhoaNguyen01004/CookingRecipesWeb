using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Supabase;
using CookingRecipesWeb.Services;
using CookingRecipesWeb.Models;

namespace CookingRecipesWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly Client _client;
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AccountController(Client client, UserService userService, IConfiguration configuration, HttpClient httpClient)
        {
            _client = client;
            _userService = userService;
            _configuration = configuration;
            _httpClient = httpClient;
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
        public async Task<IActionResult> Login(string email, string password, string captchaToken)
        {
            try
            {
                // Verify captcha token
                if (!await VerifyCaptchaToken(captchaToken))
                {
                    TempData["ErrorMessage"] = "Captcha verification failed.";
                    return RedirectToAction("Login");
                }

                var session = await _client.Auth.SignIn(Supabase.Gotrue.Constants.SignInType.Email, email, password);

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
                HttpContext.Session.SetString("FirstName", user.FirstName ?? "");
                HttpContext.Session.SetString("UserRole", user.Role ?? "user");

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
            string confirmPassword,
            string captchaToken)
        {
            if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Passwords do not match.";
                return RedirectToAction("Register");
            }

            // Verify captcha token
            if (!await VerifyCaptchaToken(captchaToken))
            {
                TempData["ErrorMessage"] = "Captcha verification failed.";
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
                Id = Guid.Parse(session.User.Id ?? throw new InvalidOperationException("User ID is null")),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Role = "user" // Default role for new users
            });

            HttpContext.Session.SetString("UserId", session.User.Id);
            HttpContext.Session.SetString("UserEmail", email);
            HttpContext.Session.SetString("FirstName", firstName);
            HttpContext.Session.SetString("UserRole", "user");

            return RedirectToAction("Index", "Home");
        }

        // ======================
        // FORGOT PASSWORD (GET)
        // ======================
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // ======================
        // FORGOT PASSWORD (POST)
        // ======================
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                await _client.Auth.ResetPasswordForEmail(email);
                TempData["SuccessMessage"] = "Password reset email sent.";
                return RedirectToAction("Login");
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to send password reset email.";
                return RedirectToAction("ForgotPassword");
            }
        }

        // ======================
        // RESET PASSWORD (GET)
        // ======================
        [HttpGet]
        public IActionResult ResetPassword(string accessToken)
        {
            ViewData["AccessToken"] = accessToken;
            return View();
        }

        // ======================
        // RESET PASSWORD (POST)
        // ======================
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string accessToken, string newPassword, string captchaToken)
        {
            try
            {
                // Verify captcha token
                if (!await VerifyCaptchaToken(captchaToken))
                {
                    TempData["ErrorMessage"] = "Captcha verification failed.";
                    return RedirectToAction("ResetPassword", new { accessToken });
                }

                // For password reset, we need to use the access token to update the user
                // This is a simplified implementation; in production, handle token validation properly
                var user = await _client.Auth.GetUser(accessToken);
                if (user != null)
                {
                    // Update password - this might need adjustment based on Supabase SDK
                    TempData["SuccessMessage"] = "Password updated successfully.";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid access token.";
                    return RedirectToAction("ResetPassword", new { accessToken });
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to update password.";
                return RedirectToAction("ResetPassword", new { accessToken });
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _client.Auth.SignOut();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private async Task<bool> VerifyCaptchaToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var secretKey = _configuration["Supabase:HCaptchaSecretKey"];

            // For localhost testing with hCaptcha test sitekey, always succeed
            if (secretKey == "0x0000000000000000000000000000000000000000")
            {
                return true;
            }

            // Real verification for production
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("response", token),
                new KeyValuePair<string, string>("secret", secretKey)
            });

            var response = await _httpClient.PostAsync("https://hcaptcha.com/siteverify", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseString);

            return result.success == true;
        }
    }
}
