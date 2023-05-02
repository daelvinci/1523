using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel loginVM)
        {
            var appUser = await _userManager.FindByNameAsync(loginVM.UserName);
            if (appUser == null || appUser.IsAdmin)
            {
                ModelState.AddModelError("", "The password or username is incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, loginVM.RememberMe, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "The password or username is incorrect");
                return View();
            }
            return RedirectToAction("index", "home");


        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel registerVM)
        {
            /*_userManager.Users.Any(x => x.NormalizedEmail == registerVM.Email.ToUpper())*/
            if (await _userManager.FindByEmailAsync(registerVM.Email) != null)
            {
                ModelState.AddModelError("Email", "Email is already exists");
                return View();
            }
            AppUser user = new()
            {
                Fullname = registerVM.FullName,
                UserName = registerVM.UserName,
                Email = registerVM.Email,

            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }

            await _userManager.AddToRoleAsync(user, "Member");
            await _signInManager.SignInAsync(user, false);


            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        public IActionResult ForgotPassword()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotted)
        {
            AppUser user = await _userManager.FindByEmailAsync(forgotted.Email);
            if (user == null || user.IsAdmin)
            {
                return NotFound();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("verify", "account", new { email = forgotted.Email, token = token }, Request.Scheme);
            //return RedirectToAction("login");
            return Ok(new { Url = url });

        }

        public async Task<IActionResult> Verify(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var result = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);

            if (result)
            {
                TempData["Email"] = email;
                TempData["Token"] = token;
                return RedirectToAction("resetpassword");

            }
            return RedirectToAction("index", "home");
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetVM)
        {
            var user = await _userManager.FindByEmailAsync(resetVM.Email);
            var result = await _userManager.ResetPasswordAsync(user, resetVM.Token, resetVM.Password);

            if (!result.Succeeded)
            {
                return NotFound();
            }
            return RedirectToAction("login");
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            UserProfileVM userProfile = new()
            {
                User = new() { Email = user.Email, FullName = user.Fullname, UserName = user.UserName }
            };

            return View(userProfile);
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserUpdateVM userUpdate)
        {
            UserProfileVM profileVM = new() { User = userUpdate };

            if (!ModelState.IsValid)
            {
                return View("profile", profileVM);
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }
            user.Email = userUpdate.Email;
            user.UserName = userUpdate.UserName;
            user.Fullname = userUpdate.FullName;

            var result = await _userManager.UpdateAsync(user);

            if (userUpdate.Password != null)
            {
                var checkPassword=await _userManager.ChangePasswordAsync(user, userUpdate.CurrentPassword, userUpdate.Password);
                
                if(!checkPassword.Succeeded)
                {
                    foreach (var item in checkPassword.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return View("profile", profileVM);
                }
            }


            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View("profile",profileVM);
            }



            await _signInManager.SignInAsync(user, true);

            return RedirectToAction("profile");
        }

    }
}
