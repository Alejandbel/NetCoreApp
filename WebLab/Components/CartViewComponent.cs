using Microsoft.AspNetCore.Mvc;
using WebLab.Domain;
using WebLab.Extensions;

namespace WebLab.Components
{
    public class CartViewComponent : ViewComponent
    {
        private readonly Cart _cart;

        public CartViewComponent(Cart cart)
        {
            _cart = cart;
        }

        public IViewComponentResult Invoke() {
            return View(_cart);
        }
    }
}
