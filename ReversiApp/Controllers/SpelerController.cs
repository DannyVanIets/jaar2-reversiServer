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
using ReversiApp.Models;
using ReversiApp.Areas.Identity.Pages.Account;
using ReversiApp.DAL;

namespace ReversiApp.Controllers
{
    public class SpelerController : Controller
    {
        private readonly ReversiContext _context;
        private readonly UserManager<Speler> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        private readonly ILogger _logger;

        public SpelerController(ReversiContext context, UserManager<Speler> userManager, RoleManager<IdentityRole> roleManager,
            ILogger<SpelerController> logger)
        {
            _context = context;
            UserManager = userManager;
            RoleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Whatever");

            List<UserAndRolesModel> users = new List<UserAndRolesModel>();

            try
            {
                foreach (var user in UserManager.Users)
                {
                    //string rol = await ReturnRoleAsync(user);

                    var rol = "Normal";
                    var isUserModerator = await UserManager.IsInRoleAsync(user, "Moderator");
                    var isAdmin = await UserManager.IsInRoleAsync(user, "Admin");

                    if (isUserModerator)
                    {
                        rol = "Moderator";
                    }
                    else if (isAdmin)
                    {
                        rol = "Admin";
                    }

                    if (User.IsInRole("Moderator") && rol == "Normal" && !user.Archived || User.IsInRole("Admin") && !user.Archived)
                    {
                        users.Add(new UserAndRolesModel
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            EmailConfirmed = user.EmailConfirmed,
                            TwoFactorEnabled = user.TwoFactorEnabled,
                            Rol = rol,
                            Kleur = user.Kleur,
                            Highscore = user.Highscore
                        });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong while going through every account: {e}");
                ViewBag.Error = e.ToString();
            }
            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchivedAsync()
        {
            List<UserAndRolesModel> users = new List<UserAndRolesModel>();

            foreach (var user in UserManager.Users)
            {
                string rol = await ReturnRoleAsync(user);

                if (User.IsInRole("Admin") && user.Archived)
                {
                    users.Add(new UserAndRolesModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        EmailConfirmed = user.EmailConfirmed,
                        TwoFactorEnabled = user.TwoFactorEnabled,
                        Rol = rol,
                        Kleur = user.Kleur,
                        Highscore = user.Highscore
                    });
                }
            }
            return View(users);
        }

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

                var user = new Speler { UserName = Input.Username, Email = Input.Email, EmailConfirmed = true, Kleur = Kleur.Geen };
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

                        #if (DEBUG)
                            _logger.LogInformation("De admin '" + loggedinUser.UserName + "' created a new account with the username '" + Input.Username + "'.");
                        #endif

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
                _logger.LogError("Er kon geen informatie gevonden worden over de gebruiker ");
                return NotFound();
            }

            string rol = await ReturnRoleAsync(userInformation);

            var speler = new UserAndRolesModel
            {
                UserId = userInformation.Id,
                UserName = userInformation.UserName,
                Email = userInformation.Email,
                EmailConfirmed = userInformation.EmailConfirmed,
                TwoFactorEnabled = userInformation.TwoFactorEnabled,
                Rol = rol,
                Kleur = userInformation.Kleur,
                Highscore = userInformation.Highscore,
                Archived = userInformation.Archived,
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

            string rol = await ReturnRoleAsync(userInformation);

            var speler = new UserAndRolesModel
            {
                UserId = userInformation.Id,
                UserName = userInformation.UserName,
                Email = userInformation.Email,
                TwoFactorEnabled = userInformation.TwoFactorEnabled,
                Rol = rol
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
                if (User.IsInRole("Admin"))
                {
                    speler.UserName = userAndRoles.UserName;
                    speler.Email = userAndRoles.Email;
                    speler.Highscore = userAndRoles.Highscore;
                }
                speler.TwoFactorEnabled = userAndRoles.TwoFactorEnabled;

                var result = await UserManager.UpdateAsync(speler);

                var claims = new List<Claim>();

                if (result.Succeeded && User.IsInRole("Admin"))
                {
                    string rol = await ReturnRoleAsync(speler);

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
                    _logger.LogInformation("De admin " + loggedinUser.UserName + " changed the account with the name " + Input.Email + ".");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToAction(nameof(Index));
            }
            // If we got this far, something failed, redisplay form
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ArchiveSpeler(string id)
        {
            var userInformation = await UserManager.FindByIdAsync(id);

            if (userInformation == null)
            {
                return NotFound();
            }

            var speler = new UserAndRolesModel
            {
                UserId = userInformation.Id,
                Archived = userInformation.Archived
            };
            return View(speler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveSpeler(UserAndRolesModel userAndRoles)
        {
            Speler speler = await UserManager.FindByIdAsync(userAndRoles.UserId);

            if (speler != null)
            {
                speler.Archived = userAndRoles.Archived;

                var result = await UserManager.UpdateAsync(speler);

                if (result.Succeeded && User.IsInRole("Admin"))
                {
                    var loggedinUser = await UserManager.GetUserAsync(HttpContext.User);
                    _logger.LogInformation("De admin " + loggedinUser.UserName + " archived the account with the email " + Input.Email + ".");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToAction(nameof(Index));
            }
            // If we got this far, something failed, redisplay form
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Delete deez nu-");
            var userInformation = await UserManager.FindByIdAsync(id);

            if (userInformation == null)
            {
                return NotFound();
            }

            string rol = await ReturnRoleAsync(userInformation);

            var speler = new UserAndRolesModel
            {
                UserId = userInformation.Id,
                UserName = userInformation.UserName,
                Email = userInformation.Email,
                EmailConfirmed = userInformation.EmailConfirmed,
                Rol = rol,
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
                        _logger.LogInformation("De admin " + loggedinUser.UserName + " deleted the account with the name " + speler.UserName + ".");

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

        public async Task<string> ReturnRoleAsync(Speler speler)
        {
            var rolNaam = "Normal";

            if (await UserManager.IsInRoleAsync(speler, "Moderator"))
            {
                rolNaam = "Moderator";
            }
            else if (await UserManager.IsInRoleAsync(speler, "Admin"))
            {
                rolNaam = "Admin";
            }
            return rolNaam;
        }
    }
}