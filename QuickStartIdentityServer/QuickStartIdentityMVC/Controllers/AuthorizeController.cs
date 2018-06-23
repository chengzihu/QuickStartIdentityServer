using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuickStartIdentityMVC.Models;

namespace QuickStartIdentityMVC.Controllers
{
    public class AuthorizeController : Controller
    {
        private JwtSettings _jwtSettings;
        public AuthorizeController(IOptions<JwtSettings> options)       //构造函数注入，拿到appsettings 里面的jwtsettings
        {
            _jwtSettings = options.Value;
        }
        [Route("api/token")]
        [HttpPost]
        public IActionResult Index(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (!(loginViewModel.Name == "lmc" && loginViewModel.PassWord == "123456"))
                return BadRequest();
            var claims = new Claim[]                //实例化一个Claim
            {
                new Claim(ClaimTypes.Name,"lmc"),
                new Claim(ClaimTypes.Role, "admin")
                //new Claim("SuperAdminOnly","true")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecreKey));  //将appsettings里面的SecreKey拿到
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);             //使用HmacSha256 算法加密
            //生成token，设置过期时间为30分钟， 需要引用System.IdentityModel.Tokens.Jwt 包
            var token = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, claims, DateTime.Now, DateTime.Now.AddMinutes(30), creds);
            //将token返回
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}