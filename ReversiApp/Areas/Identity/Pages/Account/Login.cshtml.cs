using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net;
using Newtonsoft.Json;
using ReversiApp.Models;

namespace ReversiApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<Speler> _userManager;
        private readonly SignInManager<Speler> _signInManager;
        private readonly ILogger _logger;

        public LoginModel(SignInManager<Speler> signInManager,
            UserManager<Speler> userManager,
            ILogger<LoginModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            // User already logged in? We don't want that! Send them back.
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var response = Request.Form["g-recaptcha-response"];
                //secret that was generated in key value pair
                const string secret = "6Le15uMUAAAAALTlLQwcmG6zJNrfhEnbTGvMJnHE";

                var client = new WebClient();
                var reply = client.DownloadString(
                        string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",
                    secret, response));

                var captchaResponse = JsonConvert.DeserializeObject<CaptchaReponse>(reply);

                //when response is false check for the error message
                if (!captchaResponse.Success)
                {
                    ModelState.AddModelError(string.Empty, "Captcha check failed");
                    return Page();
                }

                // Look up if the e-mail exists. If not? Invalid login attempt.
                var resultEmail = await _signInManager.UserManager.FindByEmailAsync(Input.Email);

                if (resultEmail != null)
                {
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    // This uses the username to login, so it's a roundabout solution. E-mail just makes it more secure.
                    var result = await _signInManager.PasswordSignInAsync(resultEmail.UserName, Input.Password, Input.RememberMe, true);

                    if (result.Succeeded)
                    {
                        Speler user = await _userManager.FindByEmailAsync(Input.Email);
                        if (!user.Archived)
                        {
                            _logger.LogInformation("User logged in.");
                            return LocalRedirect("/Spel");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Dit account is gearchiveerd en kan niet worden gebruikt om mee in te loggen.");
                            return Page();
                        }
                    }
                    // Test if this works.
                    else if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = "/Spel", RememberMe = Input.RememberMe });
                    }
                    else if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        _logger.LogWarning($"Invalid login attempt with the email {Input.Email}");
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
                else
                {
                    _logger.LogWarning($"Invalid login attempt with the username {Input.Email}");
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed or went wrong, redisplay form.
            return Page();
        }
    }
}
