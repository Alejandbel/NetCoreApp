using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebLab.Domain;
using WebLab.Services.BeerService;

namespace WebLab.Controllers
{
    public class CartController : Controller
    {
        private readonly IBeerService _beerService;
        private readonly Cart _cart;

        public CartController(IBeerService beerService, Cart cart)
        {
            _beerService = beerService;
            _cart = cart;
        }

        [Authorize]
        [Route("[controller]")]
        public ActionResult Index()
        {
            return View(_cart);
        }

        [Authorize]
        [Route("[controller]/add/{id:int}")]
        public async Task<ActionResult> Add(int id, string returnUrl)
        {
            var data = await _beerService.GetBeerByIdAsync(id);
            if (data.IsSuccess)
            {
                _cart.AddToCart(data.Data);
            }
            return Redirect(returnUrl);
        }

        [Authorize]
        [Route("[controller]/remove/{id:int}")]
        public async Task<ActionResult> Remove(int id, string returnUrl)
        {
            _cart.RemoveItems(id);
            return Redirect(returnUrl);
        }
    }
}
