using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using QuickStartIdentityMVC.Models;

namespace QuickStartIdentityMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(config =>
            //     {
            //         config.LoginPath = "/Account/MakeLogin";    //未认证导向登陆的页面，默认为/Account/Login
            //         config.Cookie.Name = "lmccookie";           //设置一个cookieName

            //     });
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //jwt
            //services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings")); //appsettings中读取到jwtsettings节点
            //var jwtSetting = new JwtSettings();
            //Configuration.Bind("JwtSettings", jwtSetting);
            //services.AddAuthentication(options =>
            //{                                   // 添加认证头
            //                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(jo => jo.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            //{
            //ValidIssuer = jwtSetting.Issuer,                    //使用者
            //    ValidAudience = jwtSetting.Audience,                //颁发者
            //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SecreKey)) //加密方式
            //});

            //连接数据库服务加入DI容器
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            });

            //将Identity服务加入DI容器
            services.AddIdentity<ApplicationUser, ApplicationUserRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            //设置注册密码的规则
            services.Configure<IdentityOptions>(options =>
            {
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(config =>
            {
                                config.LoginPath = "/Account/Login";    //未认证导向登陆的页面，默认为/Account/Login
                                config.Cookie.Name = "lmccookie";           //设置一个cookieName
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();//加入认证中间件
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
