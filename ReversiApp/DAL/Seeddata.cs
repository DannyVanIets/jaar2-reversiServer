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
    public static class Seeddata
    {
        public static void Initialize(IdentityContext context, UserManager<Speler> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            AddRol(roleManager, "Admin").Wait();
            AddRol(roleManager, "Moderator").Wait();
            AddRol(roleManager, "Normal").Wait();

            AddSpeler(userManager, roleManager, "dannyvanbokhorst@live.nl", "Iets-123", "Admin").Wait();
            AddSpeler(userManager, roleManager, "amonkeyeatingicecream@live.nl", "Nogwat-123", "Moderator").Wait();
            AddSpeler(userManager, roleManager, "test@live.nl", "Test-123", "Normal").Wait();
            context.SaveChanges();
        }

        private static async Task AddRol(RoleManager<IdentityRole> roleManager, string rolNaam)
        {
            var roleExists = await roleManager.FindByNameAsync(rolNaam);

            if (roleExists == null)
            {
                var role = new IdentityRole { Name = rolNaam };
                await roleManager.CreateAsync(role);
            }
        }

        private static async Task AddSpeler(UserManager<Speler> userManager, RoleManager<IdentityRole> roleManager, string email, string password, string rol)
        {
            var userExists = await userManager.FindByEmailAsync(email);
        
            //We voegen alleen een speler toe als het nog niet bestaat!
            if(userExists == null)
            {
                var claims = new List<Claim>();

                var speler = new Speler { UserName = email, Email = email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(speler, password);

                if (result.Succeeded)
                {
                    var role = await roleManager.FindByNameAsync(rol);

                    if(role != null)
                    {
                        await userManager.AddToRoleAsync(speler, role.Name);

                        claims.Add(new Claim(ClaimTypes.NameIdentifier, speler.Id));
                        claims.Add(new Claim(ClaimTypes.Role, role.Name));

                        await userManager.AddClaimsAsync(speler, claims);
                    }
                }
            }
        }
    }
}
