using EventsApp.Models.Dto;
using EventsApp.Models;
using EventsApp.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using EventsApp.Data;

namespace EventsApp.Repository.Services
{
    public class IdentityAccountService : IAccountx
    {
        private readonly UserManager<ApplicationUser> _accountManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EventsAppDbContext _context;

        public IdentityAccountService(
            UserManager<ApplicationUser> accountManager,
            SignInManager<ApplicationUser> signInManager,
            EventsAppDbContext context)
        {
            _accountManager = accountManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<AccountDto> Register(RegisterdAccountDto registerdAccountDto)
        {
            // Validate roles
            var validRoles = new List<string> { "Admin", "User" };
            if (registerdAccountDto.Role.Any(role => !validRoles.Contains(role)))
            {
                throw new ArgumentException("One or more roles are invalid.");
            }

            var account = new ApplicationUser
            {
                UserName = registerdAccountDto.Name,
                Email = registerdAccountDto.Email,
            };

            var result = await _accountManager.CreateAsync(account, registerdAccountDto.Password);

            if (result.Succeeded)
            {
                await _accountManager.AddToRolesAsync(account, registerdAccountDto.Role);
                await _context.SaveChangesAsync();

                return new AccountDto
                {
                    Id = account.Id,
                    UserName = account.UserName,
                    Role = await _accountManager.GetRolesAsync(account)
                };
            }

            throw new Exception("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        //=================================================================================================================
        public async Task<AccountDto> AccountAuthentication(string username, string password)
        {
            var account = await _accountManager.FindByNameAsync(username);

            if (account == null || !await _accountManager.CheckPasswordAsync(account, password))
            {
                throw new Exception("Invalid username or password.");
            }

            // Sign in the user using SignInManager
            var result = await _signInManager.PasswordSignInAsync(account, password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new Exception("User authentication failed.");
            }

            // Return account information including the user's roles
            return new AccountDto
            {
                Id = account.Id,
                UserName = account.UserName,
                Role = await _accountManager.GetRolesAsync(account)
            };
        }
        //=================================================================================================================

        public async Task LogOut(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                // Handle the case where username is null or empty
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty.");
            }

            var account = await _accountManager.FindByNameAsync(username);
            if (account != null)
            {
                // Perform logout logic (e.g., removing authentication cookie)
                await _signInManager.SignOutAsync();
            }
        }
    }
}
