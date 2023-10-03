using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using WebLab.IdentityServer.Models;

namespace WebLab.IdentityServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AvatarController : Controller
    {
        private IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;

        public AvatarController(IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _environment = environment;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Get()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            string GetContentType(string filePath)
            {
                var provider = new FileExtensionContentTypeProvider();
                string contentType;
                if (!provider.TryGetContentType(filePath, out contentType))
                {
                    contentType = "application/octet-stream";
                }

                return contentType;
            }

            var avatarPath = user.Image;

            if (System.IO.File.Exists(avatarPath))
            {
                string contentType = GetContentType(avatarPath);
                FileStream stream = new FileStream(avatarPath, FileMode.Open, FileAccess.Read);
                return File(stream, contentType);
            }
            else
            {
                var placeholderPath = Path.Combine(_environment.ContentRootPath, "Images", "avatar.png");

                if (System.IO.File.Exists(placeholderPath))
                {
                    string contentType = GetContentType(placeholderPath);
                    FileStream stream = new FileStream(placeholderPath, FileMode.Open, FileAccess.Read);
                    return File(stream, contentType);
                }
                else
                {
                    return NotFound("Изображение не найдено.");
                }
            }
        }

        
    }
}

