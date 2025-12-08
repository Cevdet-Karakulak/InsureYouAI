using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.Controllers
{
    public class ErrorsController : Controller
    {
        [Route("Errors/Page404")]
        public IActionResult Page404()
        {
            return View();
        }
    }
}
