using Microsoft.AspNetCore.Identity;
using ReversiApp.Areas.Identity.Data;
using ReversiApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ReversiApp.DAL
{
    public class Seeddata
    {
        private static readonly UserManager<Speler> UserManager;
        private static readonly RoleManager<IdentityRole> RoleManager;

        public static async Task InitializeAsync(IdentityContext context)
        {
            context.Database.EnsureCreated();

            await AddSpeler("DannyvanIets", "dannyvanbokhorst@live.nl", "Iets123", "Admin");
            await AddSpeler("AMonkey", "amonkeyeatingicecream@live.nl", "Nogwat123", "Moderator");
        }

        private static async Task AddSpeler(string username, string email, string password, string rol)
        { 
            var userExists = await UserManager.FindByNameAsync(username);
        
            //We voegen alleen een speler toe als het nog niet bestaat!
            if(userExists == null)
            {
                var claims = new List<Claim>();

                var user = new Speler { UserName = username, Email = email, EmailConfirmed = true };
                var result = await UserManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    var role = await RoleManager.FindByNameAsync(rol);
                    await UserManager.AddToRoleAsync(user, role.Name);

                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));

                    await UserManager.AddClaimsAsync(user, claims);
                }
            }
        }
    }
}
