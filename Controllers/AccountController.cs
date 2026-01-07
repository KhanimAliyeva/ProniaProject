using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Context;
using Pronia.Views.Account;
using System.Threading.Tasks;

namespace Pronia.Controllers
{
    public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, RoleManager<IdentityRole> _roleManager,  IConfiguration _configuration) : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var existUser = await _userManager.FindByNameAsync(vm.UserName);
            if (existUser is { })
            {
                ModelState.AddModelError("UserName", "This username is already taken.");
                return View(vm);
            }
            var existUserEmail = await _userManager.FindByEmailAsync(vm.Email);
            if (existUser is { })
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(vm);
            }

            AppUser appUser = new()
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                UserName = vm.UserName
            };

            var result = await _userManager.CreateAsync(appUser, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(vm);
            }

            await _signInManager.SignInAsync(appUser, false);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user is { })
            {
                ModelState.AddModelError("", "Invalid login attempt.");
            }

            var result = await _userManager.CheckPasswordAsync(user, vm.Password);
            if (result is { })
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(vm);
            }

            await _signInManager.SignInAsync(user, vm.RememberMe);

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> CreateRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
            await _roleManager.CreateAsync(new IdentityRole("Member"));
            await _roleManager.CreateAsync(new IdentityRole("Moderator"));

            return Ok("Roles Created");
        }

        public async Task<IActionResult> CreateAdminAndModerator()
        {
            var AdminUserVM=_configuration.GetSection("AdminUser").Get<UserVM>();
            var ModeratorUserVM = _configuration.GetSection("ModeratorUser").Get<UserVM>();

            if(AdminUserVM is not null)
            {
            AppUser adminUser = new()
            {
                FirstName = AdminUserVM.FirstName,
                LastName = AdminUserVM.LastName,
                UserName = AdminUserVM.UserName,
                Email = AdminUserVM.Email
            };
            await _userManager.CreateAsync(adminUser, AdminUserVM.Password);
            await _userManager.AddToRoleAsync(adminUser, "Admin");


            }
            if(ModeratorUserVM is not null)
            {

            AppUser moderatorUser = new()
            {
                FirstName = ModeratorUserVM.FirstName,
                LastName = ModeratorUserVM.LastName,
                UserName = ModeratorUserVM.UserName,
                Email = ModeratorUserVM.Email
            };

            await _userManager.CreateAsync(moderatorUser,ModeratorUserVM.Password);
            await _userManager.AddToRoleAsync(moderatorUser, "Moderator");
            }


            return RedirectToAction("Index", "Home");
        }
    }
}
