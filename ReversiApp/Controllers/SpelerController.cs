using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReversiApp.Areas.Identity.Data;
using ReversiApp.Areas.Identity.Pages.Account;
using ReversiApp.Models;

namespace ReversiApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Speler> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        private readonly ILogger<RegisterModel> Logger;

        public AccountController(UserManager<Speler> userManager, RoleManager<IdentityRole> roleManager,
            ILogger<RegisterModel> logger)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            Logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            List<UserAndRolesModel> users = new List<UserAndRolesModel>();
            var role = "Normal";

            foreach (var user in UserManager.Users)
            {
                if (await UserManager.IsInRoleAsync(user, "Moderator") )
                {
                    role = "Moderator";
                }
                else if (await UserManager.IsInRoleAsync(user, "Admin"))
                {
                    role = "Admin";
                }
                else if (await UserManager.IsInRoleAsync(user, "Normal"))
                {
                    role = "Normal";
                }

                users.Add(new UserAndRolesModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        EmailConfirmed = user.EmailConfirmed,
                        Rol = role,
                        Kleur = user.Kleur
                    }
                );
            }

            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AlleRollen() => View(RoleManager.Roles.ToList());

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Rollen = new SelectList(RoleManager.Roles);
            return View();
        }

        [BindProperty]
        public RegisterModel.InputModel Input { get; set; }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount()
        {
            ViewBag.Rollen = new SelectList(RoleManager.Roles);

            if (ModelState.IsValid)
            {
                var claims = new List<Claim>();

                var user = new Speler { UserName = Input.UserName, Email = Input.Email };
                var result = await UserManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    var role = await RoleManager.FindByNameAsync(Input.Rol);
                    var roleResult = await UserManager.AddToRoleAsync(user, role.Name);

                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));

                    var claimResult = await UserManager.AddClaimsAsync(user, claims);

                    if (roleResult.Succeeded && claimResult.Succeeded)
                    {
                        var loggedinUser = await UserManager.GetUserAsync(HttpContext.User);

                        Logger.LogInformation("De admin " + loggedinUser.UserName + " created a new account with the name " + Input.UserName + ".");
                        return RedirectToAction(nameof(Index));
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var userInformation = await UserManager.FindByIdAsync(id);

            if (userInformation == null)
            {
                return NotFound();
            }

            var role = "Normal";

            if (await UserManager.IsInRoleAsync(userInformation, "Moderator"))
            {
                role = "Moderator";
            }
            else if (await UserManager.IsInRoleAsync(userInformation, "Admin"))
            {
                role = "Admin";
            }

            var speler = new UserAndRolesModel
            {
                UserId = userInformation.Id,
                UserName = userInformation.UserName,
                Email = userInformation.Email,
                EmailConfirmed = userInformation.EmailConfirmed,
                Rol = role,
                Kleur = userInformation.Kleur
            };
            
            return View(speler);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var userInformation = await UserManager.FindByIdAsync(id);

            if (userInformation == null)
            {
                return NotFound();
            }

            ViewBag.Rollen = new SelectList(RoleManager.Roles);

            var role = "Normal";

            if (await UserManager.IsInRoleAsync(userInformation, "Moderator"))
            {
                role = "Moderator";
            }
            else if (await UserManager.IsInRoleAsync(userInformation, "Admin"))
            {
                role = "Admin";
            }

            var speler = new UserAndRolesModel
            {
                UserId = userInformation.Id,
                UserName = userInformation.UserName,
                Email = userInformation.Email,
                Rol = role
            };

            return View(speler);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAccount(UserAndRolesModel userAndRoles)
        {
            ViewBag.Rollen = new SelectList(RoleManager.Roles);
            Speler speler = await UserManager.FindByIdAsync(userAndRoles.UserId);

            if (speler != null)
            {
                speler.UserName = userAndRoles.UserName;
                speler.Email = userAndRoles.Email;

                var result = await UserManager.UpdateAsync(speler);
                
                var claims = new List<Claim>();

                if (result.Succeeded)
                {
                    var rol = "Normal";

                    if (await UserManager.IsInRoleAsync(speler, "Moderator"))
                    {
                        rol = "Moderator";
                    }
                    else if (await UserManager.IsInRoleAsync(speler, "Admin"))
                    {
                        rol = "Admin";
                    }

                    if (rol != userAndRoles.Rol)
                    {
                        //Eerst halen we de rollen en claims weg die bij de speler hoort
                        var alleRollen = await UserManager.GetRolesAsync(speler);
                        var alleClaims = await UserManager.GetClaimsAsync(speler);

                        await UserManager.RemoveFromRolesAsync(speler, alleRollen);
                        await UserManager.RemoveClaimsAsync(speler, alleClaims);

                        //Daarna kijken we of de rol bestaat en voegen we deze toe aan de speler en de claims.
                        var role = await RoleManager.FindByNameAsync(Input.Rol);
                        var roleResult = await UserManager.AddToRoleAsync(speler, role.Name);

                        claims.Add(new Claim(ClaimTypes.NameIdentifier, speler.Id));
                        claims.Add(new Claim(ClaimTypes.Role, role.Name));

                        var claimResult = await UserManager.AddClaimsAsync(speler, claims);
                    }

                    var loggedinUser = await UserManager.GetUserAsync(HttpContext.User);
                    Logger.LogInformation("De admin " + loggedinUser.UserName + " changed the account with the name " + Input.UserName + ".");

                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var userInformation = await UserManager.FindByIdAsync(id);

            if (userInformation == null)
            {
                return NotFound();
            }

            var role = "Normal";

            if (await UserManager.IsInRoleAsync(userInformation, "Moderator"))
            {
                role = "Moderator";
            }
            else if (await UserManager.IsInRoleAsync(userInformation, "Admin"))
            {
                role = "Admin";
            }

            var speler = new UserAndRolesModel
            {
                UserId = userInformation.Id,
                UserName = userInformation.UserName,
                Email = userInformation.Email,
                EmailConfirmed = userInformation.EmailConfirmed,
                Rol = role,
                Kleur = userInformation.Kleur
            };

            return View(speler);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            Speler speler = await UserManager.FindByIdAsync(id);

            if (speler != null)
            {
                //Eerst halen we de rollen en claims weg die bij de speler hoort
                var alleRollen = await UserManager.GetRolesAsync(speler);
                var alleClaims = await UserManager.GetClaimsAsync(speler);

                var deleteRollen = await UserManager.RemoveFromRolesAsync(speler, alleRollen);
                var deleteClaims = await UserManager.RemoveClaimsAsync(speler, alleClaims);

                if (deleteRollen.Succeeded && deleteClaims.Succeeded)
                {
                    //Daarna halen we pas de speler weg.
                    var result = await UserManager.DeleteAsync(speler);

                    if (result.Succeeded)
                    {
                        var loggedinUser = await UserManager.GetUserAsync(HttpContext.User);
                        Logger.LogInformation("De admin " + loggedinUser.UserName + " deleted the account with the name " + speler.UserName + ".");

                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }
    }
}