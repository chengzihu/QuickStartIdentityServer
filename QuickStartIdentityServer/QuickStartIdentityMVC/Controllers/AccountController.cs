using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuickStartIdentityMVC.Models;

namespace QuickStartIdentityMVC.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;//加入Identity自带的注册使用的Mananger
        private SignInManager<ApplicationUser> _signInManager;//加入Identity自带的登录 使用的Manager

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// 注册页面
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public IActionResult Register(string returnUrl="/Home/Index")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel,
            string returnUrl="/Home/Index")
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)//model 验证
            {
                ApplicationUser identityUser = new ApplicationUser
                {
                    Email=registerViewModel.Email,
                    UserName=registerViewModel.Email,
                    NormalizedEmail=registerViewModel.Email
                };

                var result = await _userManager.CreateAsync(identityUser,registerViewModel.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(identityUser,new AuthenticationProperties { IsPersistent=true});
                    return Redirect(returnUrl);
                }
                else
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("",err.Description);
                    }
                }
            }

            return View();
        }

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public IActionResult Login(string returnUrl="/Home/Index")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(RegisterViewModel loginViewModel,
            string returnUrl = "/Home/Index")
        {
            ViewData["ReturnUrl"] = returnUrl;
            var loginUser = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if (loginUser == null)
            {
                return View();
            }
            await _signInManager.SignInAsync(loginUser,new AuthenticationProperties { IsPersistent=true});
            return Redirect(returnUrl);
        }

        public async Task<IActionResult> MakeLogin()
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name,"lmc"),
                new Claim(ClaimTypes.Role,"admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                 new ClaimsPrincipal(claimsIdentity),
                 new AuthenticationProperties
                 {
                     IsPersistent = true,                              //cookie过期时间设置为持久
                     ExpiresUtc = DateTime.UtcNow.AddSeconds(20)      //设置过期20秒
                 });
            return Ok();
        }

        public async Task<IActionResult> Logout()
        {
            //        await HttpContext.SignOutAsync(
            //CookieAuthenticationDefaults.AuthenticationScheme);
            await _signInManager.SignOutAsync();
            //return Ok();
            return Redirect("/Home/Index");
        }
    }

    //覆盖默认验证
    public class MyCookieTestAuthorize : CookieAuthenticationEvents
    {
        public override Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            return base.ValidatePrincipal(context);
        }
    }
}