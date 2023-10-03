using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebLab.IdentityServer.Models;

namespace WebLab.IdentityServer.Pages.Register
{
	[AllowAnonymous]
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<RegisterModel> _logger;
        private readonly IWebHostEnvironment _environment;

        public RegisterModel(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ILogger<RegisterModel> logger,
			IWebHostEnvironment environment)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
            _environment = environment;
        }

		[BindProperty]
		public InputModel Input { get; set; }

		public string ReturnUrl { get; set; }

		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		public class InputModel
		{
			[Required]
			[EmailAddress]
			[Display(Name = "Email")]
			public string Email { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
			[DataType(DataType.Password)]
			[Display(Name = "Password")]
			public string Password { get; set; }

			[Display(Name = "Image")]
			public IFormFile Image { get; set; }


			[DataType(DataType.Password)]
			[Display(Name = "Confirm password")]
			[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
			public string ConfirmPassword { get; set; }
		}

		public async Task OnGetAsync(string returnUrl = null)
		{
			ReturnUrl = returnUrl;
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, EmailConfirmed = true };
				var result = await _userManager.CreateAsync(user, Input.Password);


                if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");

                    if (Input.Image != null)
                    {
                        var imagePath = GetImagePath(user.Id);
                        await SaveImageAsync(imagePath);
						user.Image = imagePath;
						await _userManager.UpdateAsync(user);
                    }
					

                    await _signInManager.SignInAsync(user, isPersistent: false);
					return LocalRedirect(returnUrl);
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}

		private string GetImagePath(string id)
		{
            var ext = Path.GetExtension(Input.Image.FileName);
            var fileName = Path.ChangeExtension(id, ext);
            return Path.Combine(_environment.ContentRootPath, "Images", fileName);
        }
		private async Task SaveImageAsync(string path)
		{
			using var stream = System.IO.File.OpenWrite(path);
			await Input.Image.CopyToAsync(stream);
		}
	}
}
