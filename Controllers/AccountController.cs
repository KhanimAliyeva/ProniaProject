using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Abstraction;
using Pronia.Context;
using Pronia.Views.Account;
using System.Threading.Tasks;

namespace Pronia.Controllers;

public class AccountController(UserManager<AppUser> _userManager, IEmailService _emailService, SignInManager<AppUser> _signInManager, RoleManager<IdentityRole> _roleManager, IConfiguration _configuration) : Controller
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

        await _userManager.AddToRoleAsync(appUser, "Member");

        await SendConfirmationEmail(appUser);

        TempData["SuccessMessage"] =  " Please check your email to confirm your account.";

        return RedirectToAction("Login");
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

        if (!user.EmailConfirmed)
        {
            ModelState.AddModelError("", "Please confirm your email before logging in.");
            await SendConfirmationEmail(user);
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
        var AdminUserVM = _configuration.GetSection("AdminUser").Get<UserVM>();
        var ModeratorUserVM = _configuration.GetSection("ModeratorUser").Get<UserVM>();

        if (AdminUserVM is not null)
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
        if (ModeratorUserVM is not null)
        {

            AppUser moderatorUser = new()
            {
                FirstName = ModeratorUserVM.FirstName,
                LastName = ModeratorUserVM.LastName,
                UserName = ModeratorUserVM.UserName,
                Email = ModeratorUserVM.Email
            };

            await _userManager.CreateAsync(moderatorUser, ModeratorUserVM.Password);
            await _userManager.AddToRoleAsync(moderatorUser, "Moderator");
        }


        return RedirectToAction("Index", "Home");
    }

    private async Task SendConfirmationEmail(AppUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //string url = @$"https://localhost:7097/Account/ConfirmEmail?token={token}&userId={user.Id} ";

        string url = Url.Action("ConfirmEmail", "Account", new { token = token, userId = user.Id }, Request.Scheme) ?? string.Empty;

        string emailBody = @$"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <title>Email Confirmation</title>
</head>
<body style=""margin:0; padding:0; background-color:#f6f6f6; font-family: Arial, sans-serif;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""padding:40px 0;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" 
                       style=""background:#ffffff; border-radius:8px; box-shadow:0 4px 12px rgba(0,0,0,0.08); padding:40px;"">

                    <!-- IMAGE -->
                    <tr>
                        <td align=""center"" style=""padding-bottom:20px;"">
<img 
src=""https://res.cloudinary.com/dgpf0kufs/image/upload/v1767897155/dark_cnavfr.png""
    alt=""Pronia Logo""
    width=""120""
    style=""display:block;"">

                        </td>
                    </tr>

                    <!-- TITLE -->
                    <tr>
                        <td align=""center"" style=""padding-bottom:16px;"">
                            <h2 style=""margin:0; color:#2e2e2e;"">
                                Confirm Your Email Address
                            </h2>
                        </td>
                    </tr>

                    <!-- TEXT -->
                    <tr>
                        <td align=""center"" style=""padding-bottom:30px; color:#6f6f6f; font-size:15px; line-height:1.6;"">
                            Thank you for registering at <strong>Pronia</strong> 🌿  
                            <br />
                            Please <b>{user.FirstName} {user.LastName}</b> confirm your email address to activate your account.
                        </td>
                    </tr>

                    <!-- BUTTON -->
                    <tr>
                        <td align=""center"" style=""padding-bottom:25px;"">
                            <a href=""{url}""
                               style=""
                                   background-color:#a8d26d;
                                   color:#ffffff;
                                   padding:14px 36px;
                                   text-decoration:none;
                                   border-radius:30px;
                                   font-weight:bold;
                                   font-size:14px;
                                   display:inline-block;"">
                                CONFIRM EMAIL
                            </a>
                        </td>
                    </tr>

                    <!-- LINK -->
                    <tr>
                        <td align=""center"" style=""font-size:13px; color:#999;"">
                            If the button doesn’t work, click the link below:
                            <br /><br />
                            <a href=""{url}"" style=""color:#a8d26d; word-break:break-all;"">
                                {url}
                            </a>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>
";

        await _emailService.SendEmailAsync(user.Email, "Confirm your email", emailBody);
    }

    public async Task<IActionResult> ConfirmEmail(string token, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return BadRequest();
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return BadRequest();
        }

        await _signInManager.SignInAsync(user, false);
        return RedirectToAction("Index", "Home");
    }


}
