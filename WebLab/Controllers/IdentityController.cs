using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebLab.Controllers
{
    public class IdentityController : Controller
    {
        public async Task Login()
        {
            await HttpContext.ChallengeAsync(
                "oidc",
                new AuthenticationProperties
                {
                RedirectUri =
                Url.Action("Index", "Home") });
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("access_token");
            await HttpContext.SignOutAsync("oidc",
            new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            });
        }
    }
}

