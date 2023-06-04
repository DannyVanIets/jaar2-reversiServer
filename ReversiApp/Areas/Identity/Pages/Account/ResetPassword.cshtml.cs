using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ReversiApp.Models;

namespace ReversiApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<Speler> _userManager;

        public ResetPasswordModel(UserManager<Speler> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
        }

        public enum PasswordScore
        {
            TooShort = 0,
            VeryWeak = 1,
            Weak = 2,
            Medium = 3,
            Strong = 4,
            VeryStrong = 5
        }

        public static PasswordScore CheckStrength(string password)
        {
            int score = 0;
            var regexItem = new Regex("^[a-zA-Z0-9]*$");
            var regexNumbers = new Regex("[0-9]");

            if (password.Length < 12)
                return PasswordScore.TooShort;
            if (password.Length < 15)
                return PasswordScore.VeryWeak;

            if (password.Length >= 16)
                score++;
            if (password.Length >= 20)
                score++;
            if (regexNumbers.IsMatch(password))
                score++;
            if (password.Any(char.IsUpper) && password.Any(char.IsLower))
                score++;
            if (password.Any(ch => !char.IsLetterOrDigit(ch)))
                score++;

            return (PasswordScore)score;
        }

        public IActionResult OnGetPasswordChange(string password)
        {
            if (password == null)
            {
                return new JsonResult(PasswordScore.TooShort.ToString());
            }
            var result = CheckStrength(password).ToString();
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            if (Input.Password.Length > 128)
            {
                ModelState.AddModelError(string.Empty, "Het wachtwoord moet korter zijn dan 128 karakters!");
            }
            else
            {
                var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
                if (result.Succeeded)
                {
                    return RedirectToPage("./ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }
    }
}
