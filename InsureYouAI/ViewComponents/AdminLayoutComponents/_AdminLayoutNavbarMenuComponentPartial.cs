using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponenets.AdminLayoutComponents;

public class _AdminLayoutNavbarMenuComponentPartial:ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
