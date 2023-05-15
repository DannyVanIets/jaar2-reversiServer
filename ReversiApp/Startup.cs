using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReversiApp.DAL;
using ReversiApp.Models;
using ReversiApp.Services;

namespace ReversiApp
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
            var lockoutOptions = new LockoutOptions()
            {
                AllowedForNewUsers = true,
                DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5),
                MaxFailedAccessAttempts = 5
            };

            services.AddControllersWithViews();
            services.AddIdentity<Speler, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
                .AddEntityFrameworkStores<ReversiContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();
            services.AddDbContext<ReversiContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ReversiContextConnection")));
            services.AddMvc();

            //for the captcha
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //For sending an e-mail
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddSingleton<IConfiguration>(Configuration);
            //services.Configure<AuthMessageSenderOptions>(Configuration);

            //For redirecting always to the login page and email inactivity timeout
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
            });

            services.AddControllers().AddNewtonsoftJson();

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ReversiContext context, UserManager<Speler> userManager, RoleManager<IdentityRole> roleManager, ILoggerFactory loggerFactory)
        {
            Seeddata.Initialize(context, userManager, roleManager);

            // Through this we don't need to use a middleware application.
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Remove("X-Powered-By");
                await next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Spel}/{action=Index}/{id?}").RequireAuthorization();
                endpoints.MapRazorPages();
            });

            //loggerFactory.AddFile("Logs/Information/InformationLog-{Date}.txt", LogLevel.Information);
            //loggerFactory.AddFile("Logs/Warnings/WarningLog-{Date}.txt", LogLevel.Warning);
        }
    }
}