using EventsApp.Models.Dto;
using EventsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventsApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountx _accountService;
        public AccountController(IAccountx accountService)
        {
            _accountService = accountService;
        }
        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterdAccountDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var account = await _accountService.Register(model);
                    // Handle success (e.g., redirect to login or home page)
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
           
                try
                {
                    // Authenticate the user using the AccountService
                    var account = await _accountService.AccountAuthentication(model.UserName, model.Password);

                    // If authentication is successful, redirect to the home page or dashboard
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    // If authentication fails, show an error message to the user
                    ModelState.AddModelError("", ex.Message);
                }
            

            // If the model state is not valid or authentication fails, return the login view with the model
            return View(model);
        }

        // GET: Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated) // Check if the user is authenticated
            {
                var username = User.Identity.Name; // Get the current user's name
                await _accountService.LogOut(username);
            }
            else
            {
                // Handle case when user is not authenticated
                return RedirectToAction("Login");
            }

            return RedirectToAction("Login");
        }

    }
}
