using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ReversiApp.Models;

namespace ReversiApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Speler> _signInManager;
        private readonly UserManager<Speler> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<Speler> userManager,
            SignInManager<Speler> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Gebruikersnaam")]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            //[DataType(DataType.EmailAddress)] Not sure if necessary?
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Wachtwoord")]
            [DataType(DataType.Password)]
            //[StringLength(maximumLength: 130, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Wachtwoord bevestigen")]
            //[StringLength(maximumLength: 130, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Rol")]
            public string Rol { get; set; }

            [Required]
            [Display(Name = "Akkoord met privacy statement")]
            public bool AgreedToPrivacy { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
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
            if(password == null)
            {
                return new JsonResult(PasswordScore.TooShort.ToString());
            }
            var result = CheckStrength(password).ToString();
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
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
                }

                if (!Input.AgreedToPrivacy)
                {
                    ModelState.AddModelError(string.Empty, "Je moet met de privacy statement akkoord gaan als je de website wilt gebruiken met een account.");
                }

                if(Input.Password.Length > 128)
                {
                    ModelState.AddModelError(string.Empty, "Het wachtwoord moet korter zijn dan 128 karakters!");
                }
                else
                {
                    var user = new Speler { UserName = Input.Username, Email = Input.Email };
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Beste meneer/mevrouw,<br><br>U kunt uw account activeren door <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>hier</a> te klikken. Als u niet zich heeft opgegeven om mee te doen met Reversi, kunt u deze e-mail negeren.<br><br>Met vriendelijke groet,<br><br>DannyvanIets");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
