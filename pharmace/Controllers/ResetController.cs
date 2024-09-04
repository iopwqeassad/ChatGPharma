using Microsoft.AspNetCore.Mvc;

namespace pharmace.Controllers
{
    public class ResetController : Controller
    {
        public IActionResult resetpass()
        {
            return View();
        }
    }
}
