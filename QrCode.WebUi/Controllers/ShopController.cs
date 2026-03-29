using Microsoft.AspNetCore.Mvc;

namespace QrCode.WebUi.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
