using Microsoft.AspNetCore.Mvc;

namespace WebLab.Components
{
	public class CartViewComponent : ViewComponent
	{
		public IViewComponentResult Invoke() => View();
	}
}
