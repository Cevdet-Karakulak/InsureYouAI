using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponenets.AdminLayoutComponents;

public class _AdminLayoutNavbarComponentPartial:ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
