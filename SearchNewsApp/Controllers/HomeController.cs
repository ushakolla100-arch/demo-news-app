using Microsoft.AspNetCore.Mvc;

namespace SearchNewsApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("SearchNewsApp is running! Go to /News/Index to search.");
        }
    }
}
