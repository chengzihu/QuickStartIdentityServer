using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace QuickStartIdentityMVC.Controllers
{
    public class AccountController : Controller
    {
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
            await HttpContext.SignOutAsync(
    CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

    public IActionResult Index()
        {
            return View();
        }
    }
}