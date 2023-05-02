using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Areas.AdminPanel.ViewModels;
using WebApplication1.DAL;
using WebApplication1.Models;


namespace WebApplication1.Areas.AdminPanel.Controllers
{
    
    [Area("AdminPanel")]

 
    public class AccountController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(PustokDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Create()
        {
            AppUser appUser = new()
            {
                UserName = "MemberElvin",
                IsAdmin = true
            };
            var result = await _userManager.CreateAsync(appUser, "Member123");

            if (!result.Succeeded)
            {
                return Ok(result.Errors);
            }
            await _userManager.AddToRoleAsync(appUser, "Member");

            return Ok(result);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginVM loginVM, string returnUrl)
        {
            AppUser user = await _userManager.FindByNameAsync(loginVM.UserName);

            if (user == null|| !user.IsAdmin) 
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View();
            }

            var result =await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, true);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View();
            }

            
            return returnUrl!=null?Redirect(returnUrl):RedirectToAction("index", "dashboard");
        }

        public async Task<IActionResult> CreateRole()
        {
            IdentityRole role1 = new() { Name = "Member" };
            IdentityRole role2 = new() { Name = "Admin" };
            IdentityRole role3 = new() { Name = "SuperAdmin" };

            await _roleManager.CreateAsync(role1);//resulti var succeed dir mi deyilmi bax
            await _roleManager.CreateAsync(role2);
            await _roleManager.CreateAsync(role3);

            return Ok();
        }
    }
}
