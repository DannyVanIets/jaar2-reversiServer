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
using SendGrid;

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
            // Can change the signin, user, identity and password options, like the length of a password.
            // Password needs: digit, non-alphanumeric character, unique char, lower-case, upper-case. Length must be above 6.
            services.AddIdentity<Speler, IdentityRole>(options =>
            {
                //options.SignIn.RequireConfirmedAccount = true; // Uitgezet voor nu, doordat het e-mail versturen niet werkt momenteel.
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 12;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<ReversiContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();
            services.AddDbContext<ReversiContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ReversiContextConnection")));

            services.AddCors(options =>
            {
                options.AddPolicy(name: "_allowedSpecificOrigins",
                                policy =>
                                {
                                    policy.WithOrigins("http://localhost:50433").AllowAnyMethod();
                                });
            });

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

            // https://learn.microsoft.com/en-us/aspnet/core/security/authorization/razor-pages-authorization?view=aspnetcore-7.0
            //For redirecting always to the login page and email inactivity timeout
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            });

            services.AddAntiforgery(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.HttpOnly = true;
                options.Cookie.MaxAge = TimeSpan.FromDays(30);
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.Name = "__Host-.AspNetCore.Session";
            });

            services.AddControllers().AddNewtonsoftJson();

            services.AddRazorPages(options =>
            {
                options.Conventions.AllowAnonymousToFolder("/Identity");
                options.Conventions.AllowAnonymousToFolder("/Home");
                options.Conventions.AllowAnonymousToFolder("/Shared");
                options.Conventions.AuthorizeFolder("/Spel");
                options.Conventions.AuthorizeFolder("/Speler");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ReversiContext context, UserManager<Speler> userManager, RoleManager<IdentityRole> roleManager)
        {
            Seeddata.Initialize(context, userManager, roleManager);

            // Through this we don't need to use a middleware application.
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-XSS-Protection", "0");
                context.Response.Headers.Add("Content-Disposition", "attachment; filename='api.json'");
                context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
                context.Response.Headers.Add("Referrer-Policy", "same-origin");
                context.Response.Headers.Add("Content-Security-Policy",
                    "default-src 'self' ; " +
                    "frame-src https://www.google.com/ ; " +
                    "connect-src 'self' wss: localhost: localhost:5043 wss://localhost:44370/ReversiApp/ http://localhost:50433 https: ; " +
                    "img-src 'self' data: ; " +
                    "frame-ancestors 'self'; ; " +
                    "style-src 'self' 'unsafe-inline' ; " +
                    "script-src 'self' 'unsafe-inline' https: ;");
                    //"script-src 'self' https: https://www.google.com/recaptcha/api.js https://www.gstatic.com/recaptcha/releases/wqcyhEwminqmAoT8QO_BkXCr/recaptcha__nl.js ;");
                context.Response.Headers.Remove("X-Powered-By");
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-AspNet-Version");
                context.Response.Headers.Remove("X-AspNetMvc-Version");
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

            app.UseCors("_allowedSpecificOrigins");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    //pattern: "{controller=Spel}/{action=Index}/{id?}").RequireAuthorization();
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}